namespace CDK
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false)]
    public class GetComponentAttribute : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false)]
    public class GetComponentInChildrenAttribute : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false)]
    public class GetComponentInParentAttribute : System.Attribute { }
}