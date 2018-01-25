using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
	UnitData _unitData;
	UnitView _unitView;
}

public class UnitData
{
	
}

public class UnitView
{
	
}

//单位的总控制器，用于协调一切子控制器
public class UnitCtrler
{
	
}
//运动控制器，自身不具有决策能力，仅仅实现了运动功能的实现。
//使用时由其他 Controller进行调用
//该控制器会有重载，以实现不同的运动方式。
public class MotionCtrler
{
	
}

//用于在战斗状态时控制单位的行为，会与多个控制器协同执行
//单位的 决策能力由战斗AI模块实现，AI的决策结果 传递给该模块并调用相应的控制器。
public class BattleCtrler
{

}

//普通攻击的控制器
public class AttachCtrler
{
	
}
//技能释放
public class SkillCastCtrler
{
	
}

//单位空闲时的控制器
public class IdelCtrler
{
	
}

//ai决策结果：
// 逃避	： 运动控制器
// 追击	： 运动控制器
// 攻击	： 战斗控制器
// 释放技能
// 放弃	：角色控制器 ，转换为idel状态


//会有多个动画控制器
public class XXXAnimCtrler
{
	
}
