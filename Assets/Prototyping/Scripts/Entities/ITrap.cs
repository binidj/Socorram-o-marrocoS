using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototyping.Scripts.Entities
{
    public interface ITrap
    {
        public bool isPlacing {get; set;}

        public void Trigger();
    }
}

