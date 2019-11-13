using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T: Singleton<T>, new()
{
    private static T m_instance ;

    private static readonly object synObject = new object();

    public static T Instance
    {
        get
        {
            if(m_instance == null) //double checking
            {
                lock(synObject)
                {
                    if(m_instance == null)
                    {
                        m_instance = new T();
                    }
                }
            }
            return m_instance;
        }
    }   
   
}
