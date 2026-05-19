//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;


//public interface IAnimatorInfo { }

//public class AnimatorInfo : IAnimatorInfo
//{
//    public Animator animator;
//    public UnityAction actions;
//}

//public class AnimatorInfo<T> : IAnimatorInfo
//{
//    public Animator animator;
//    public UnityAction<T> actions;
//}

//public class AnimatorManager : BaseManager<AnimatorManager>
//{
//    private Dictionary<string, AnimatorInfo> animatorInfos = new Dictionary<string, AnimatorInfo>();

//    public void AddAnimator(string name, Animator animator)
//    {
//        if (animatorInfos.ContainsKey(name))
//        {
//            if (animatorInfos[name].animator == null)
//                animatorInfos[name].animator = animator;
//            else
//                return;
//        }
//        else
//            animatorInfos.Add(name, new AnimatorInfo { animator = animator });
//    }

//    public void SetAnimator<T>(string name, string str, T value, UnityAction)
//    {
//        if (!animatorInfos.ContainsKey(name)) return;

//        if (animatorInfos.ContainsKey(name))
//        {
//            if (typeof(T) == typeof(float))

//        }
//    }
//}
