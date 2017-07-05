using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyRideSharing.Models;

namespace MyRideSharing.Security
{
    public static class SessionPersister
    {
        public static string Email
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionvar = HttpContext.Current.Session["email"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["email"] = value;
            }
        }


        public static int UserId
        {
            get
            {
                if (HttpContext.Current == null)
                    return -1;
                var sessionvar = HttpContext.Current.Session["userId"];
                if (sessionvar != null)
                    return Int32.Parse(sessionvar.ToString());
                return -1;
            }
            set
            {
                HttpContext.Current.Session["userId"] = value;
            }
        }

        public static string StudentId
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                var sessionvar = HttpContext.Current.Session["studentId"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["studentId"] = value;
            }
        }

        public static string NationalId
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                var sessionvar = HttpContext.Current.Session["nationalId"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["nationalId"] = value;
            }
        }

        public static string FirstName
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionvar = HttpContext.Current.Session["firstName"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["firstName"] = value;
            }
        }
        public static string LastName
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionvar = HttpContext.Current.Session["lastName"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["lastName"] = value;
            }
        }


    }
}