using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerPlayModeTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void PlayerPlayModeTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerPlayModeTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerGrabsAllInstantiatedItems()
    {
        List<GameObject> instances = new List<GameObject>();
        for (int i = 0; i < 10; ++i)
        {
            instances.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Item")));
            instances.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Bot")));
        }

        GameObject playerObj = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        Player pScript = playerObj.GetComponent<Player>();
        pScript.TagsToCheckDistance = new string[] { "BotsGroup", "ItemsGroup" };

        Assert.IsTrue(pScript.GrabItems().Length == instances.Count);

        yield return null;
    }
}
