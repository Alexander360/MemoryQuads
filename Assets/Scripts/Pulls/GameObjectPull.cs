using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;

public class GameObjectPull<T> where T : Component
{
    private readonly GameObject Container;
    private readonly T prefab;
    private readonly int addingCount;
    private readonly Stack<T> instances;

    public GameObjectPull(GameObject container, T prefabObj, int startCount, int addingCount = 5)
    {
        prefab = prefabObj;
        this.addingCount = addingCount;
        Container = container;
        instances = new Stack<T>(startCount);
        addInstances(startCount);
    }

    public T GetObject()
    {
        if (instances.Count == 0)
        {
            addInstances(addingCount);
        }
        return instances.Pop();
    }

    public void ReleaseObject(T objectToRelease)
    {
        objectToRelease.gameObject.AppendTo(Container);
        objectToRelease.gameObject.SetActive(false);
        instances.Push(objectToRelease);
    }

    public void ClearPull()
    {
        while (instances.Count > 0)
        {
            GameObject.Destroy(instances.Pop());
        }
    }


    private void addInstances(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T instance = PrefabHelper.Intantiate(prefab, Container);
            instance.gameObject.SetActive(false);
            instances.Push(instance);
        }
    }

}
