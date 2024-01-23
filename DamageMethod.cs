﻿//------------------------------------------------------------------------------
/*
 * TowerD (2023)
 * User: Kurt Gav
 * Date: 11/4/2023
 * Time: 3:05 pm
 * 
 */
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense
{
    internal abstract class DamageMethod
    {
        public abstract void deal(List<Enemy> enemies, double delta_t);
        //deal damage and/or status effects to enemies
    }

    internal class NormalDamage : DamageMethod
    {
        private double damage;

        public NormalDamage(double damage)
        {
            this.damage = damage;
        }
        public override void deal(List<Enemy> enemies, double delta_t)
        {
            foreach(var e in enemies)
            {
                e.dealtDamage(damage);
            }
        }
    }

    internal class ContinuousDamage : DamageMethod
    {
        private double damageSpeed; //damage per second

        public ContinuousDamage(double damageSpeed)
        {
            this.damageSpeed = damageSpeed;
        }

        public override void deal(List<Enemy> enemies, double delta_t)
        {
            foreach (var e in enemies)
            {
                e.dealtDamage(damageSpeed * delta_t);
            }
        }
    }

    internal class FreezeSingleDamage : DamageMethod
    {
        private double damage;

        public FreezeSingleDamage(double damage)
        {
            this.damage = damage;
        }
        public override void deal(List<Enemy> enemies, double delta_t)
        {
            foreach (var e in enemies)
            {
                e.dealtDamage(damage);
                e.dealtStatusEffect(StatusEffect.Frozen, 3.0);
            }
        }
    }
}
