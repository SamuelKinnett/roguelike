using UnityEngine;
using System.Collections;

public class Node {

    public Node parent;
    public int x;
    public int y;
    public float f;
    public float g;
    public float h;
    public int parentCount;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Initilise(int x,int y,float f,float g, float h)
    {
        parentCount = 0;
        x = this.x;
        y = this.y;
        f = this.f;
        g = this.g;
        h = this.h;
    }
    public void SetParent(Node parent)
    {
        parentCount++;
        parent = this.parent;
    }
    public Node GetParent()
    {
        return parent;
    }
    public int GetParentCount()
    {
        return parentCount;
    }
    public void SetF()
    {
        f = g+h;
    }
    public float GetF()
    {
        return f;
    }
    public void SetG(float g)
    {
        this.g = g;
    }
    public float GetG()
    {
        return g;
    }
    public void SetH(float h)
    {
        this.h = h;
    }
    public float GetH()
    {
        return h;
    }
    public void SetX(int x)
    {
        this.x = x;
    }
    public int GetX()
    {
        return x;
    }
    public void SetY(int y)
    {
        this.y = y;
    }
    public int GetY()
    {
        return y;
    }
}
