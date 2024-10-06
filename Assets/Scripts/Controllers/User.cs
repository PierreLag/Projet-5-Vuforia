using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string login;
    public string accessLevel;
    public UserRole role;

    public static User FromJSON(string json)
    {
        User user = JsonUtility.FromJson<User>(json);
        
        switch (user.accessLevel)
        {
            case "USER":
                user.role = UserRole.USER;
                break;
            case "SUPPORT":
                user.role = UserRole.SUPPORT;
                break;
            case "ADMIN":
                user.role = UserRole.ADMIN;
                break;
            case "SUPERADMIN":
                user.role = UserRole.SUPERADMIN;
                break;
        }

        return user;
    }

    public User(string username)
    {
        this.login = username;
        this.role = UserRole.USER;
    }

    public string GetUsername()
    {
        return login;
    }

    public UserRole GetRole()
    {
        return role;
    }

    public override string ToString()
    {
        return "Username : " + login + ", role : " + role;
    }
}
