using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganInfor 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public OrganInfor(string _id, string _name, string _description)
    {
        Id = _id;
        Name = _name;
        Description = _description;
    }
}
