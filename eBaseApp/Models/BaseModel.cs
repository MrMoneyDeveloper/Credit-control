using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Text.Json.Serialization;

namespace eBaseApp.Models
{
    public class BaseModel : IAuditable
    {
        [Column(Order = 1)]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Column(Order = 101)]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Column(Order = 102)]
        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; }



        [Column(Order = 103)]
        [Display(Name = "Is Locked")]
        public bool? IsLocked { get; set; }

        [Column(Order = 104)]
        public int? CreatedBySystemUserId { get; set; }
        [ForeignKey("CreatedBySystemUserId")]
        [Display(Name = "Created By")]
        [ScriptIgnore]
        public SystemUser CreatedBySystemUser { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created On")]
        [Column(Order = 105)]
        public DateTime? CreatedDateTime { get; set; }

        [Column(Order = 106)]
        public int? ModifiedBySystemUserId { get; set; }
        [ForeignKey("ModifiedBySystemUserId")]
        [Display(Name = "Modified By")]
        [ScriptIgnore]
        [JsonIgnore]
        public SystemUser ModifiedBySystemUser { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Modified On")]
        [Column(Order = 107)]
        public DateTime? ModifiedDateTime { get; set; }

        // JK.20160909a - Include encrypted Ids
        [NotMapped]
        public string Data { get; set; }


        [NotMapped]
        public string ColorCode { get; set; }

        // JK.20160909a - Include encrypted Ids
        [NotMapped]
        public List<string> DataList { get; set; }

        /// <summary>
        /// Calculates the percentage completed of the object.
        /// How many properties are filled in with a valid value.
        /// </summary>
        /// <returns></returns>
        public int PercentageComplete()
        {
            int complete = 0;
            int total = 0;
            string value = string.Empty;

            PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var p in props.Where(p => p.GetType() != typeof(Nullable)))
            {
                switch (p.PropertyType.ToString())
                {
                    case "System.String":
                        value = p.GetValue(this) != null ? p.GetValue(this).ToString() : string.Empty;
                        total++;
                        if (!string.IsNullOrEmpty(value.Trim())) complete++;
                        break;
                    case "System.Int32":
                        value = p.GetValue(this) != null ? p.GetValue(this).ToString() : "0";
                        total++;
                        if (value != "0") complete++;
                        break;
                    case "System.DateTime":
                        value = p.GetValue(this) != null ? p.GetValue(this).ToString() : DateTime.MinValue.ToString();
                        total++;
                        if (value != DateTime.MinValue.ToString()) complete++;
                        break;
                }
            }

            return (complete * 100) / total;
        }

        /// <summary>
        /// Compares this object to another object.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns></returns>
        public bool Compare(object o)
        {
            if (o == null) return false;
            if (o.GetType() != this.GetType()) return false;

            var props =
                this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var prop in props)
            {
                switch (prop.PropertyType.ToString())
                {
                    case "System.String":
                    case "System.Int32":
                    case "System.DateTime":
                        if (null != prop.GetValue(this))
                        {
                            if (!prop.GetValue(this, null).ToString().Equals(prop.GetValue(o, null).ToString(), StringComparison.OrdinalIgnoreCase))
                                return false;
                        }

                        break;
                }
            }

            return true;
        }
    }

}