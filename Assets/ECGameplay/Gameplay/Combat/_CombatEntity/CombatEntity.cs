﻿using System;
using cfg.Status;
using UnityEngine;

namespace ECGameplay
{
    public class CombatEntity : Entity
    {
        // 普攻行为
        public AttackAction AttackAction { get; private set; }
        // 格挡行为
        public BlockAction BlockAction { get; private set; }
        // 效果分配行为
        // public EffectAssignAction EffectAssignAction { get; private set; }
        // 伤害行为
        public DamageAction DamageAction { get; private set; }
        
        // 普攻能力
        public AttackAbility AttackAbility { get; private set; }
        
        

        public override void Awake()
        {
            AddComponent<AttributeComponent>();
            AddComponent<ActionPointComponent>();
            AddComponent<StatusComponent>();
            
            AttackAction = AttachAction<AttackAction>();
            BlockAction = AttachAction<BlockAction>();
            DamageAction = AttachAction<DamageAction>();
            // EffectAssignAction = AttachAction<EffectAssignAction>();

            AttackAbility = AttachAbility<AttackAbility>(1);
        }

    #region 行动点

        public void ListenActionPoint(ActionPointType actionPointType, Action<Entity> action)
        {
            GetComponent<ActionPointComponent>().AddListener(actionPointType, action);
        }

        public void UnListenActionPoint(ActionPointType actionPointType, Action<Entity> action)
        {
            GetComponent<ActionPointComponent>().RemoveListener(actionPointType, action);
        }

        public void TriggerActionPoint(ActionPointType actionPointType, Entity action)
        {
            GetComponent<ActionPointComponent>().TriggerActionPoint(actionPointType, action);
        }

    #endregion

    #region 受伤/治疗

        public void ReceiveDamage(IActionExecution actionExecution)
        {
            var damageAction = actionExecution as DamageActionExecution;
            if (damageAction == null)
                return;
            GetComponent<AttributeComponent>().HealthPoint.MinusBaseValue(damageAction.Damage);
        }

    #endregion

    #region 添加能力/状态

        public T AttachAbility<T>(object config = null) where T : Entity, IAbility
        {
            return config == null ? AddChild<T>() : AddChild<T>(config);
        }
        
        public T AttachAction<T>(object config = null) where T : Entity, IAction
        {
            var action = config == null ? AddChild<T>() : AddChild<T>(config);
            action.Enable = true;
            return action;
        }

        public StatusAbility AttachStatus(object config)
        {
            return GetComponent<StatusComponent>().AttachStatus(config);
        }
        
        public bool TryGetStatus(int id, out StatusAbility status)
        {
            status = null;
            var comp = GetComponent<StatusComponent>();
            if (comp.TypeIdStatuses.ContainsKey(id))
            {
                if (comp.TypeIdStatuses[id].Count > 0) 
                {
                    status = comp.TypeIdStatuses[id][0];
                    return true;
                }
            }

            return false;
        }
    #endregion
    }
}