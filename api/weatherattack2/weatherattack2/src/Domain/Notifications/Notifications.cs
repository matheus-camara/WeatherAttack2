﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using weatherattack2.src.Domain.Entities;

namespace weatherattack2.src.Domain.Notifications.User
{
    public static class UserNotifications
    {
        public static readonly Notification InvalidName = new Notification("UN-001", "Invalid name");
        public static readonly Notification InvalidEmail = new Notification("UN-002", "Invalid email");
        public static readonly Notification InvalidUsername = new Notification("UN-003", "Invalid Username");
    }
}