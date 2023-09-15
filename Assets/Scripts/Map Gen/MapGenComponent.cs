/*
 * This the Abstract component that will get process by Map Gen Manager to generate the level
 */

using System.Collections;
using UnityEngine;

public abstract class MapGenComponent : MonoBehaviour, IMapGen
{
    public virtual void Initialize() {

    }

    public abstract IEnumerator Process();
}
