using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string login;
    public UserRole role;

    public static User FromJSON(string json)
    {
        return JsonUtility.FromJson<User>(json);
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
