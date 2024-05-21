using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Enemies;

public class EnemyRagdoll : MonoBehaviour
{

    [TitleGroup("Component")]
    public Animator Animator;
    public List<IKHarmAnimation> IKHarmAnimations = new List<IKHarmAnimation>();
    public Waving Waving;

    private Rigidbody[] rigidbodies;

    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRagdoll(false);
    }

    public void SetRagdoll(bool isActive)
    {
        Animator.enabled = !isActive;
        Waving.enabled = !isActive;
        foreach (var iKHarmAnimation in IKHarmAnimations)
        {
            iKHarmAnimation.enabled = !isActive;
        }
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = !isActive;
            if (isActive)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }
    }

    [Button("Activate ragdoll")]
    public void ActivateRagdoll()
    {
        SetRagdoll(true);
    }

    [Button("Deactivate ragdoll")]
    public void DeactivateRagdoll()
    {
        SetRagdoll(false);
    }
}
