using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TestID))]
public class MyIDEditor : Editor {
    private static Dictionary<long, TestID> IDS;
    private static bool inited = false;

    public override void OnInspectorGUI () {
        init();
        //base.OnInspectorGUI ();
        TestID id = (TestID)target;
        // If the id is assigned and just not in the table
        if (id.ID > 0 && IDS.ContainsKey(id.ID) && IDS[id.ID] == null) {
            if (!TryToAdd(id)) {
                Debug.LogError("Could not add ID to dictionary.");
            }
        } else if (id.ID <= 0 || IDS.ContainsKey(id.ID) && IDS[id.ID] != id && IDS[id.ID] != null) {
            // If the ID is unassigned or the ID is assigned incorrectly
            var newid = GetLatestID();
            id.ID = newid;
            if (!TryToAdd(id)) {
                Debug.LogError("Could not add ID to dictionary.");
            }
        }
        EditorGUILayout.LabelField("LS_ID", id.ID.ToString());
    }

    public static long GetLatestID () {
        long id = 1;
        // There are smarter ways to do this by keeping a cached last id
        // and starting from there, but this will keep the namespace more dense
        // and even running through 1000 objects won't take that much time.
        // If it starts causing slowdown consider adding most recent ID cache.
        while (IDS.ContainsKey(id) && IDS[id] != null) {
            id++;
        }
        return id;
    }

    // Attempts to add the id to the dictionary of IDs in the scene.
    // Returns true if successful and false if it would overwrite another
    // valid id.
    public static bool TryToAdd (TestID id) {
        if (IDS.ContainsKey(id.ID)) {
            if (IDS[id.ID] == null) { 
                IDS[id.ID] = id;
            } else {
                // The id was present and filled in.
                return false;
            }
        } else {
            IDS.Add(id.ID, id);
        }
        return true;
    }

    public static void init () {
        if (inited) {
            return;
        }
        if (IDS == null) {
            IDS = new Dictionary<long, TestID>();
        }
        var ids = FindObjectsOfType<TestID>();
        for (int i = 0; i < ids.Length; i++) {
            var id = ids[i];
            if (IDS.ContainsKey(id.ID) && IDS[id.ID] == id) {
                continue;
            } 
            if (!TryToAdd(id)) {
                // We have a missassigned ID. Get new id
                id.ID = GetLatestID();
                if (!TryToAdd(id)) {
                    Debug.LogError("Could not add ID to dictionary.");
                }
            }
        }
        inited = true;
    }
}
