using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    // total width of spawning area in X axis(eg.width of 40 would be -20 to 20)
    public float spawnAreaWidth = 40f;
    //total height of spawning area in Y axis
    public float spawnAreaHeight = 30f;
    // the number of rocks to spawn
    public int numToSpawn = 10;

    public GameObject[] rocks; // create and fill when Spawn is called
    public GameObject rockPrefab;

    // Start is called before the first frame update
    void Start()
    {

        // #3 Respawn on key press
        // #3b nicely clean up all the spawned rocks first
        // #3c Make sure you also delete existing astroids so you don't have dangling pointers
        // #3d Make sure array doesn't have nulls (and while you are doing this, make sure you don't break the array iteration)
        Spawn();

        //RunUnitTests();
    }

    // creates numToSpawn rocks in the scene randomly but within the spawn area. 
    public void Spawn()
    {

        // check for existing array
        // First is simple case where array doesn't exist or is empty, so we create it. 
        if( rocks == null || rocks.Length == 0)
        {
            // New Array as before
            rocks = new GameObject[numToSpawn];
            AddRocksToArray(numToSpawn, 0);
        }
        else
        {
            // In this case, rocks already has some entries so we have to manually copy over the entries first. 
            int newArraySize = rocks.Length + numToSpawn; // size is sum of current + new
            int previousLength = rocks.Length;

            GameObject[] newArray = new GameObject[newArraySize];
            // Copy rocks over from old array
            for(int i = 0; i < rocks.Length; i++)
            {
                newArray[i] = rocks[i];
            }

            // get rid of old array
            rocks = newArray; // object reference to the array itself. 

            // careful as this is hardcoded to use the rocks array so it must have
            // the new, expanded array reference BEFORE we call it. 
            AddRocksToArray(numToSpawn, previousLength);
        }
    }

    // This function can add rocks to an array with a given starting position
    // Does NOT check for array out of bounds or any other errors so be careful. 
    // By making this a function we can handle both cases (partially full and empty arrays)
    private void AddRocksToArray(int rocksToAdd, int startingIndex)
    {
        // Create a new array

        for (int i = startingIndex; i < rocks.Length; i++)
        {
            // #1 add random variance
            rocks[i] = Instantiate(rockPrefab); // #5 Change to Rock type and use Generic tag. 

            rocks[i].transform.position = GetSpawnPosition();
            // #2 add to parent object (optional, depends if parent moves but helps in scene organization, so attach it to something!)
        }
    }

    private Vector3 GetSpawnPosition()
    {
        // Set position: 
        float halfWidth = spawnAreaWidth / 2f;
        float halfHeight = spawnAreaHeight / 2f;
        float xRandomPosition = Random.Range(-halfWidth, halfWidth);
        float yRandomPosition = Random.Range(-halfHeight, halfHeight);

        // I like to cache before returning as it makes debugging later a little easier.  
        Vector3 newSpawnPosition = new Vector3(xRandomPosition, yRandomPosition, 0);
        return newSpawnPosition;
    }

    // Destroys all rocks in the array and sets it to a new length 0 array. 
    public void ClearAllRocks()
    {
        for (int i = 0; i < rocks.Length; i++)
        {
            Destroy(rocks[i]);
        }

        // create a new but empty array
        rocks = new GameObject[0];
    }

    // See Random.Range for a random value. 

    

    


    // Update is called once per frame
    void Update()
    {
        
    }

    // Just making sure this all works
    void RunUnitTests()
    {
        Spawn();

        Debug.Log("Array Contains: " + rocks.Length + " entries (Should be " + numToSpawn);

        ClearAllRocks();

        Debug.Log("Array Contains: " + rocks.Length + " entries (Should be " + 0);

        Spawn();
        Spawn();
        Spawn();

        Debug.Log("Array Contains: " + rocks.Length + " entries (Should be " + numToSpawn * 3);

        ClearAllRocks();

        Debug.Log("Array Contains: " + rocks.Length + " entries (Should be " + 0);
    }
}
