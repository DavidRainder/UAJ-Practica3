using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField]
    Sprite image;
    public float parallaxFactor;
    public float layerDepth;
    public bool tilesInX = true;
    public bool tilesInY = false;

    private List<GameObject> tiles;
    private Vector3 lastCameraPosition;
    private Camera camReference;
    public void Initialize(Camera camera)
    {
        camReference = camera;
        tiles = new List<GameObject>();
        //Generar el tile central
        tiles.Add(CreateTile(new Vector3(0,0,layerDepth)));

        if (tilesInX)
        {
            for(int i = -1 ; i <= 1; i+= 2)
            {
                tiles.Add(CreateTile(new Vector3(i * image.bounds.size.x, 0, layerDepth)));
            }
        }
        if (tilesInY)
        {
            for(int i = -1 ; i <= 1; i+= 2)
            {
                tiles.Add(CreateTile(new Vector3(0, i * image.bounds.size.y, layerDepth)));
            }
        }
        if (tilesInX && tilesInY)
        {
            for(int i = -1 ; i <= 1; i+= 2)
            {
                for(int j = -1 ; j <= 1; j+= 2)
                    tiles.Add(CreateTile(new Vector3(i * image.bounds.size.x, j * image.bounds.size.y, layerDepth)));
                
            }
        }

    }
    private GameObject CreateTile(Vector3 offset)
    {
        GameObject tile = new GameObject();
        var sR = tile.AddComponent<SpriteRenderer>();
        sR.sprite = image;
        
        //resize the sprite so it fits the screen

        var width = sR.sprite.bounds.size.x;
        var height = sR.sprite.bounds.size.y;
        
        var worldScreenHeight = camReference.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        
        
        tile.transform.localScale = new Vector3(
            (float)(worldScreenWidth / width),
            (float)(worldScreenHeight / height),
            1);
        
        tile.transform.position = new Vector3(offset.x * tile.transform.localScale.x, offset.y * tile.transform.localScale.y, layerDepth);
        
        sR.sortingOrder = -4;
        
        return tile;

    }
    public void Move(Vector3 position)
    {
        Vector3 delta = (position - lastCameraPosition) * parallaxFactor;
        
        foreach(GameObject tile in tiles)
        {
            lastCameraPosition = position;
            tile.transform.position += new Vector3(delta.x * (tilesInX ? 1f : 0f), delta.y * (tilesInY ? 1f : 0f), 0) * parallaxFactor;

            if (Mathf.Abs(tile.transform.position.x - position.x) > (2 * image.bounds.size.x * tile.transform.localScale.x))
            {
                if (tile.transform.position.x > position.x)
                    tile.transform.position -= new Vector3(3 * image.bounds.size.x  * tile.transform.localScale.x, 0, 0);
                else
                    tile.transform.position += new Vector3(3 * image.bounds.size.x  * tile.transform.localScale.x, 0, 0);
            }


            if (Mathf.Abs(tile.transform.position.y - position.y) > (2 * image.bounds.size.y  * tile.transform.localScale.y))
            {
                if (tile.transform.position.y > position.y)
                    tile.transform.position -= new Vector3(0, 3 * image.bounds.size.y * tile.transform.localScale.y, 0);
                else
                    tile.transform.position += new Vector3(0, 3 * image.bounds.size.y * tile.transform.localScale.y, 0);
            }
            
            if (!tilesInX && !tilesInY)
            {
                tile.transform.position = new Vector3(position.x , position.y, layerDepth);
            }
            if (tilesInX && !tilesInY)
            {
                tile.transform.position = new Vector3(tile.transform.position.x , position.y, layerDepth);
            }
            if (tilesInY && !tilesInX)
            {
                tile.transform.position = new Vector3(position.x , tile.transform.position.y, layerDepth);
            }
        }
    }
}
