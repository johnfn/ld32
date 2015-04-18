using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class PooledObject
{
    public GameObject obj;

    public bool isAlive;
}

public class ObjectPool
{
    private Func<GameObject> _makeObject;

    private List<PooledObject> _pool = new List<PooledObject>();

    public ObjectPool(Func<GameObject> makeObject)
    {
        _makeObject = makeObject;
    }

    public GameObject SpawnObject()
    {
        var deadObject = _pool.FirstOrDefault(t => !t.isAlive);

        if (deadObject != null)
        {
            deadObject.isAlive = true;
            deadObject.obj.GetComponent<SpriteRenderer>().enabled = true;

            return deadObject.obj;
        }

        var newPooledObject = new PooledObject
        {
            obj = _makeObject(),
            isAlive = true
        };

        _pool.Add(newPooledObject);

        return newPooledObject.obj;
    }

    public void KillObject(GameObject obj)
    {
        var soonToBeDeadObject = _pool.First(t => t.obj == obj);

        soonToBeDeadObject.isAlive = false;
        soonToBeDeadObject.obj.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void KillAllObjects()
    {
        foreach (var obj in _pool)
        {
            KillObject(obj.obj);
        }
    }
}
