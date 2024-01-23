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
using System.Drawing;

namespace TowerDefense
{
    internal class Pig : Tower
    {
        public Pig(Tile position)
        {
            name = "Canon";
            texture = Resource1.pig;
            this.position = position;
            cost = 100;
            maxCooldown = 1.0;

            range = 3.0;
            selectMethod = new SniperSelect(range, position);
            damageMethod = new NormalDamage(5.0);
        }

        public override void select(List<Enemy> enemies, double delta_t)
        {
            cooldownSelect(enemies, delta_t);
        }
    }

    internal class Snowball : Tower
    {
        public Snowball(Tile position)
        {
            name = "Ice Tower";
            texture= Resource1.snowball;
            this.position = position;
            cost = 120;
            maxCooldown = 0.75;

            range = 3.5;
            selectMethod = new SniperSelectNotFreeze(range, position);
            damageMethod = new FreezeSingleDamage(5.0);
        }

        public override void select(List<Enemy> enemies, double delta_t)
        {
            cooldownSelect(enemies, delta_t);
        }
    }

    internal class Napoleon : Tower
    {
        public Napoleon(Tile position)
        {
            name = "Mage Tower";
            texture = Resource1.napoleon;
            this.position = position;
            cost = 150;
            maxCooldown = 1.0;

            range = 2.5;
            selectMethod = new AreaSelect(range, position);
            damageMethod = new NormalDamage(3.0);
        }

        public override void select(List<Enemy> enemies, double delta_t)
        {
            cooldownSelect(enemies, delta_t);
        }
    }

    internal class Boxer : Tower
    {
        public Boxer(Tile position)
        {
            name = "Artillery Tower";
            texture = Resource1.boxer;
            this.position = position;
            cost = 150;
            maxCooldown = 3.0;

            range = 4.0;
            selectMethod = new SniperSelect(range, position);
            damageMethod = new NormalDamage(15.0);
        }

        public override void select(List<Enemy> enemies, double delta_t)
        {
            cooldownSelect(enemies, delta_t);
        }
    }

    internal class Hedgehog : Tower
    {
        public Hedgehog(Tile position)
        {
            name = "Archer Tower";
            texture = Resource1.hedgehog;
            this.position = position;
            cost = 120;

            range = 2.5;
            selectMethod = new AreaSelect(range, position);
            damageMethod = new ContinuousDamage(2.0);
        }

        public override void select(List<Enemy> enemies, double delta_t)
        {
            continuousSelect(enemies);
        }
    }
}