using eBaseApp.DataAccessLayer;
using eBaseApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace eBaseApp.Helpers
{
    public class DatabaseHelper
    {
        private readonly eServicesDbContext _context;
        public DatabaseHelper(eServicesDbContext context)
        {
            _context = context;
        }
        public void ActionTypes()
        {
            List<ActionType> types = new List<ActionType>();
            types.Add(new ActionType { Name = "CC-Pre-Termination Note(1816)", Description = "CC-Pre-Termination Note(1816)" });
            foreach(ActionType type in types)
            {
                type.Key = String.Format("a_{0}", type.Name.ToLower().Replace(" ", "_"));
                if(_context.ActionTypes.FirstOrDefault(a=>a.Key == type.Key) == null)
                {
                    _context.ActionTypes.Add(type);
                }
                _context.SaveChanges();
            }
        }
    }
}