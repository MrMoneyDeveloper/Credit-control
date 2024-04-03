using eBaseApp.Models;
using eBaseApp.Models.Audits;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace eBaseApp.DataAccessLayer
{
    public class eServicesDbContext : IdentityDbContext<SystemIdentityUser>
    {
        public eServicesDbContext()
            : base("eServicesDbContext", throwIfV1Schema: false)
        {
        }
        public static eServicesDbContext Create() => new eServicesDbContext();
        public SystemUser CurrentSystemUser { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<AppSettingAudit> AppSettingAudits { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<StatusAudit> StatusAudits { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<SystemUserAudit> SystemUserAudits { get; set; }

        //application models 
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<AllocatedAccount> AllocatedAccounts { get; set; }
        public DbSet<RoundRobinQueue> RoundRobinQueues { get; set; }
        public DbSet<ResponsibilityType> ResponsibilityTypes { get; set; }
        public DbSet<Technician> Technicians { get; set; }

        public DbSet<JobCardBalance> JobCardBalances { get; set; }

        public override int SaveChanges()
        {
            var changeCount = 0;
            string entityState = string.Empty;
            try
            {
                var transactionDateTime = DateTime.Now;
                var changeSet = ChangeTracker.Entries();
                int? currentUserId = CurrentSystemUser != null ? CurrentSystemUser.Id : (int?)null;
                string tableName = string.Empty;
                List<AuditEnitity> singleTableAudit = new List<AuditEnitity>();
                List<AuditEnitity> perTableAudits = new List<AuditEnitity>();
                this.Database.CommandTimeout = 180;

                foreach (var entry in changeSet.Where(e => e.Entity is IAuditable))
                {
                    Type entryEntityType = entry.Entity.GetType();
                    string primaryKeyName = GetPrimaryKeyProperty(entryEntityType);
                    int primaryKey = Convert.ToInt32(entryEntityType.GetProperty(primaryKeyName).GetValue(entry.Entity));
                    entityState = entry.State.ToString();
                    tableName = GetTableName(entryEntityType);

                    ((IAuditable)entry.Entity).ModifiedBySystemUserId = currentUserId;
                    ((IAuditable)entry.Entity).ModifiedDateTime = transactionDateTime;

                    switch (entry.State)
                    {
                        case EntityState.Added:

                            ((IAuditable)entry.Entity).CreatedBySystemUserId = currentUserId;
                            ((IAuditable)entry.Entity).CreatedDateTime = transactionDateTime;
                            ((BaseModel)entry.Entity).IsActive = true;
                            ((BaseModel)entry.Entity).IsDeleted = false;
                            ((BaseModel)entry.Entity).IsLocked = false;

                            foreach (var propertyName in entry.CurrentValues.PropertyNames)
                            {
                                singleTableAudit.Add(new AuditEnitity()
                                {
                                    Audit = new Audit()
                                    {
                                        Action = entityState,
                                        TableName = tableName,
                                        PrimaryKey = primaryKey,
                                        ColumnName = propertyName,
                                        CurrentValue = Convert.ToString(entry.CurrentValues[propertyName]),
                                        OriginalValue = string.Empty,
                                        AuditBySystemUserId = currentUserId,
                                        AuditDateTime = transactionDateTime,
                                        IPAddress = GetIP() ?? HttpContext.Current.Request.UserHostAddress,

                                    },
                                    Entity = entry.Entity
                                });
                            }

                            perTableAudits.Add(new AuditEnitity()
                            {
                                Audit = new Audit()
                                {
                                    Action = entityState
                                },
                                Entity = entry.Entity
                            });

                            break;
                        case EntityState.Modified:
                        case EntityState.Deleted:
                            ((IAuditable)entry.Entity).CreatedBySystemUserId = ((IAuditable)entry.Entity).CreatedBySystemUserId;
                            ((IAuditable)entry.Entity).CreatedDateTime = ((IAuditable)entry.Entity).CreatedDateTime;

                            foreach (var propertyName in entry.CurrentValues.PropertyNames)
                            {
                                if ((entry.CurrentValues[propertyName] != null && entry.GetDatabaseValues()[propertyName] != null) && !entry.CurrentValues[propertyName].Equals(entry.GetDatabaseValues()[propertyName]))
                                    singleTableAudit.Add(new AuditEnitity()
                                    {
                                        Audit = new Audit()
                                        {
                                            Action = entityState,
                                            TableName = tableName,
                                            PrimaryKey = primaryKey,
                                            ColumnName = propertyName,
                                            CurrentValue = Convert.ToString(entry.CurrentValues[propertyName]),
                                            OriginalValue = Convert.ToString(entry.OriginalValues[propertyName]),
                                            AuditBySystemUserId = currentUserId,
                                            AuditDateTime = transactionDateTime,
                                            IPAddress = GetIP() ?? HttpContext.Current.Request.UserHostAddress,
                                        },
                                        Entity = entry.Entity
                                    });
                            }

                            perTableAudits.Add(new AuditEnitity()
                            {
                                Audit = new Audit()
                                {
                                    Action = entityState
                                },
                                Entity = entry.Entity
                            });

                            break;
                    }
                }

                // var changeCount = base.SaveChanges();
                changeCount = base.SaveChanges();

                // JK.20140902a - Auditing is processed here.
                foreach (var auditEnitity in singleTableAudit)
                {
                    if (auditEnitity.Entity != null)
                    {
                        Type entryEntityType = auditEnitity.Entity.GetType();
                        string primaryKeyName = GetPrimaryKeyProperty(entryEntityType);
                        int primaryKey =
                            Convert.ToInt32(entryEntityType.GetProperty(primaryKeyName).GetValue(auditEnitity.Entity));

                        auditEnitity.Audit.PrimaryKey = primaryKey;
                    }

                    // Single table audit.
                    Audits.Add(auditEnitity.Audit);
                }

                foreach (var e in perTableAudits)
                {
                    SaveAudit(e);
                }

                //Save audit trail.
                base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    string entityName = validationErrors.Entry.Entity.GetType().Name;
                    string controller = HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
                    string action = HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");

                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Helpers.SecurityHelper.LogError(new Exception(string.Format("Entity Name: {0} State: {1} Controller: {2} Action: {3} Property: {4} Error: {5}", entityName, entityState,
                        controller, action, validationError.PropertyName, validationError.ErrorMessage)), null);
                    }
                }
            }
            return changeCount;
        }
        /// <summary>
        /// Saves the audits for per table auditing.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void SaveAudit(AuditEnitity entry)
        {
            try
            {
                var name = entry.Entity.GetType().Name;
                var space = entry.Entity.GetType().Namespace;
                var type = Type.GetType(string.Format("{0}.Audits.{1}Audit", space, name));

                object audit = null;

                if (type != null)
                {
                    audit = Activator.CreateInstance(type);

                    var props =
                        entry.Entity.GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    audit.GetType().GetProperty("Action").SetValue(audit, entry.Audit.Action);

                    foreach (var prop in props)
                    {
                        if (prop.CanWrite && prop.CanRead)
                            switch (prop.PropertyType.ToString())
                            {
                                case "System.String":
                                case "System.string":
                                case "System.Int32":
                                case "System.Nullable`1[System.Int32]":
                                case "System.Int64":
                                case "System.bool":
                                case "System.Boolean":
                                case "System.Nullable`1[System.Boolean]":
                                case "System.DateTime":
                                case "System.Nullable`1[System.DateTime]":
                                    if (prop.GetValue(entry.Entity, null) != null)
                                    {
                                        try
                                        {
                                            audit.GetType().GetProperty(prop.Name).SetValue(audit, prop.GetValue(entry.Entity));
                                        }
                                        catch (Exception)
                                        {
                                            //
                                        }

                                    }
                                    break;
                            }
                    }

                    switch (name)
                    {
                        case "AppSetting":
                            AppSettingAudits.Add((AppSettingAudit)audit);
                            break;
                        case "SystemUser":
                            SystemUserAudits.Add((SystemUserAudit)audit);
                            break;
                        case "Status":
                            StatusAudits.Add((StatusAudit)audit);
                            break;
                    }
                }
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        private static Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();
        public string GetIP()
        {
            string Str = "";
            Str = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(Str);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }
        /// <summary>
        /// Gets the entity set.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Entity type not found in GetTableName</exception>
        private EntitySetBase GetEntitySet(Type type)
        {
            if (!_mappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;
                string typeName = ObjectContext.GetObjectType(type).Name;
                var es =
                    octx.MetadataWorkspace.GetItemCollection(DataSpace.SSpace).GetItems<EntityContainer>().SelectMany(
                        c => c.BaseEntitySets.Where(e => e.Name == typeName)).FirstOrDefault();

                if (es == null)
                    throw new ArgumentException("Entity type not found in GetTableName", typeName);

                _mappingCache.Add(type, es);
            }

            return _mappingCache[type];
        }

        /// <summary>
        /// Gets the name of the table of the entity type.
        /// Used for auditing entities.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private string GetTableName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);
            return string.Format("{0}", es.MetadataProperties["Table"].Value);
        }

        /// <summary>
        /// Gets the primary key property of an entity type.
        /// Used to get the value of the primary key for auditing.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private string GetPrimaryKeyProperty(Type type)
        {
            EntitySetBase es = GetEntitySet(type);
            return es.ElementType.KeyMembers[0].Name;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // JK.20140902a - Include this to remove cascade deletions.
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<SystemUser>()
                .HasOptional(f => f.CreatedBySystemUser)
                .WithMany()
                .HasForeignKey(f => f.CreatedBySystemUserId);

            modelBuilder.Entity<SystemUser>()
                .HasOptional(f => f.ModifiedBySystemUser)
                .WithMany()
                .HasForeignKey(f => f.ModifiedBySystemUserId);
        }
    }
}