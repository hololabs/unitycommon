using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using System.Linq.Expressions;

public static class Deb
{
    public static void Var<T>(Expression<Func<T>> expr)
    {
        var body = (MemberExpression)expr.Body;
        Debug.Log(body.Member.Name + " = " + body.Member);
    }
}
