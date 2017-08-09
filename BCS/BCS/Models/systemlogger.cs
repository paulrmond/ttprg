using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCS.Models;

namespace BCS.Models
{
    public class systemlogger
    {


        //ZONE GROUP
        ApplicationDbContext context = new ApplicationDbContext();
        BCS_Context db = new BCS_Context();
        systemlogs syslogs = new systemlogs();

        public int ZGCode { get; set; }

        public string Message { get; set; }

        public string AreaAccessed { get; set; }

        public string User { get; set; }


        public void LogInfo(string User, string AreaAccessed, string Message)
        {

            this.Message = Message;
            this.AreaAccessed = AreaAccessed;
            this.User = User;



            var currentUserId = context.Users.Where(s => s.UserName == User).Select(s => s.ZoneGroup).FirstOrDefault();
            var GName = context.Users.Where(s => s.UserName == User).Select(s => s.GivenName).FirstOrDefault();
            var LName = context.Users.Where(s => s.UserName == User).Select(s => s.LastName).FirstOrDefault();
            var FullName = GName + " " + LName;
            //var currentUserZoneGroup = context.Users.SingleOrDefault(m => m.Id == currentUserId).ZoneGroup;
            //var currentUserDivision = context.Users.SingleOrDefault(m => m.Id == currentUserId).Division;
            //var ZoneName = db.ZoneGroup.Single(m => m.ZoneGroupCode == currentUserZoneGroup).ZoneGroupName;

            syslogs.UserName = FullName;
            syslogs.ZoneGroupCode = currentUserId;
            syslogs.AreaAccessed = AreaAccessed;
            syslogs.loglevel = "INFO";
            syslogs.remarks = Message;
            syslogs.timestamp = DateTime.Now;


            db.systemlogs.Add(syslogs);
            db.SaveChanges();
        }

        public void LogError(string User, string AreaAccessed, string Message)
        {

            this.Message = Message;
            this.AreaAccessed = AreaAccessed;
            this.User = User;


            var currentUserId = context.Users.Where(s => s.UserName == User).Select(s => s.ZoneGroup).FirstOrDefault();
            var GName = context.Users.Where(s => s.UserName == User).Select(s => s.GivenName).FirstOrDefault();
            var LName = context.Users.Where(s => s.UserName == User).Select(s => s.LastName).FirstOrDefault();
            var FullName = GName + " " + LName;
            //var currentUserZoneGroup = context.Users.SingleOrDefault(m => m.Id == currentUserId).ZoneGroup;
            //var currentUserDivision = context.Users.SingleOrDefault(m => m.Id == currentUserId).Division;
            //var ZoneName = db.ZoneGroup.Single(m => m.ZoneGroupCode == currentUserZoneGroup).ZoneGroupName;

            syslogs.UserName = FullName;
            syslogs.ZoneGroupCode = currentUserId;
            syslogs.AreaAccessed = AreaAccessed;
            syslogs.loglevel = "ERROR";
            syslogs.remarks = Message;
            syslogs.timestamp = DateTime.Now;

            db.systemlogs.Add(syslogs);
            db.SaveChanges();
        }

        public void LogWarning(string User, string AreaAccessed, string Message)
        {
            var currentUserId = context.Users.Where(s => s.UserName == User).Select(s => s.ZoneGroup).FirstOrDefault();
            var GName = context.Users.Where(s => s.UserName == User).Select(s => s.GivenName).FirstOrDefault();
            var LName = context.Users.Where(s => s.UserName == User).Select(s => s.LastName).FirstOrDefault();
            var FullName = GName + " " + LName;
            //var currentUserZoneGroup = context.Users.SingleOrDefault(m => m.Id == currentUserId).ZoneGroup;
            //var currentUserDivision = context.Users.SingleOrDefault(m => m.Id == currentUserId).Division;
            //var ZoneName = db.ZoneGroup.Single(m => m.ZoneGroupCode == currentUserZoneGroup).ZoneGroupName;

            syslogs.UserName = FullName;
            syslogs.ZoneGroupCode = currentUserId;
            syslogs.AreaAccessed = AreaAccessed;
            syslogs.AreaAccessed = AreaAccessed;
            syslogs.loglevel = "WARNING";
            syslogs.remarks = Message;
            syslogs.timestamp = DateTime.Now;


            db.systemlogs.Add(syslogs);
            db.SaveChanges();
        }







    }
}