﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowerDefense
{
    enum StatusEffect
    {
        Frozen = 0,
        Poisoned = 1,
        TotalCount = 2
    }

    enum EnemyType
    {
        Basic = 0,
        Leopard = 1,
        Toad = 2,
        Snail = 3,
        Mouse = 4,
        Scorpion = 5
    }

    internal abstract partial class Enemy
    {
        protected double maxHP;
        protected double HP;
        protected int attack; //attack power (HP lost if reached base)
        protected double defaultSpeed; //movement speed (default, tile)
        protected double speed; //current movement speed (after status effect, tile)
        protected int reward; //money given when killed
        protected List<double> status; //status effect timers

        //for movement calculation
        protected List<Tile> path;
        protected int movingStage = 0; //if enemy position between Tile i and i+1 of path, this should be i

        protected double pos_x;
        protected double pos_y; //position (pixel, top-left)
        protected double distance = 0.0; //distance from the start (to measure the furthest away)

        public int Attack { get { return attack; } }
        public int Reward { get { return reward; } }
        public double Pos_x { get { return pos_x; } }
        public double Pos_y { get { return pos_y; } }
        public double Distance { get { return distance; } }

        public bool dead()
        {
            double eps = 1e-5;
            if (HP <= eps) return true;
            return false;
        }
        public bool reachedBase()
        {
            if (movingStage >= path.Count-1) return true;
            return false;
        }
        public bool frozen()
        {
            if (status[(int)StatusEffect.Frozen] > 0.0) return true;
            return false;
        }

        public void initPath(List<Tile> path)
        {
            this.path = path;
        }
        public void initPosition(Tile startTile)
        {
            pos_x = ((double)startTile.x) * GridParams.TileSize;
            pos_y = ((double)startTile.y) * GridParams.TileSize;
        }
        public abstract void move(double delta_t); //calculate movement
        public abstract void statusEffect(double delta_t); //calculate status effects

        public abstract void dealtDamage(double val); //tower deal damage to enemy
        public abstract void dealtStatusEffect(StatusEffect type, double val); //tower deal status effect to enemy
    
        protected void defaultMove(double delta_t)
        {
            if (this.reachedBase()) return;

            double dirX = ((double)(path[movingStage + 1].x - path[movingStage].x)) * GridParams.TileSize;
            double dirY = ((double)(path[movingStage + 1].y - path[movingStage].y)) * GridParams.TileSize;

            pos_x += speed * dirX * delta_t;
            pos_y += speed * dirY * delta_t;
            distance += speed * GridParams.TileSize * delta_t;

            if (!InSegment(pos_x, pos_y, path[movingStage], path[movingStage + 1]))
            {
                movingStage++;
                
                if (this.reachedBase()) return;

                double exceededX = Math.Abs(pos_x - ((double)path[movingStage].x) * GridParams.TileSize);
                double exceededY = Math.Abs(pos_y - ((double)path[movingStage].y) * GridParams.TileSize);
                dirX = (double)(path[movingStage + 1].x - path[movingStage].x);
                dirY = (double)(path[movingStage + 1].y - path[movingStage].y);
                initPosition(path[movingStage]);
                pos_x += dirX * exceededX;
                pos_y += dirY * exceededY;
            }
        }

        private bool InSegment(double x, double y, Tile a, Tile b)
        {
            double ax = ((double)a.x) * GridParams.TileSize;
            double ay = ((double)a.y) * GridParams.TileSize;
            double bx = ((double)b.x) * GridParams.TileSize;
            double by = ((double)b.y) * GridParams.TileSize;
            if (a.x != b.x)
            {
                if (x < Math.Min(ax, bx)) return false;
                if (x > Math.Max(ax, bx)) return false;
                return true;
            }
            else
            {
                if (y < Math.Min(ay, by)) return false;
                if (y > Math.Max(ay, by)) return false;
                return true;
            }
        }

        protected void defaultStatusEffect(double delta_t)
        {
            speed = defaultSpeed;

            if (status[(int)StatusEffect.Frozen] > 0.0)
            {
                speed = 0.5 * defaultSpeed; //values subject to change
            }
            if (status[(int)StatusEffect.Poisoned] > 0.0)
            {
                dealtDamage(10.0 * Math.Min(delta_t, status[(int)StatusEffect.Poisoned])); //values subject to change
            }

            for (int i=0; i<status.Count; i++)
            {
                if (status[i] > 0.0) status[i] -= delta_t;
            }
        }

        protected void defaultDealtDamage(double val)
        {
            HP -= val;
            displayHurt();
        }

        protected void defaultDealtStatusEffect(StatusEffect type, double val)
        {
            status[(int)type] = val;
        }

        public static Enemy produceEnemy(int enemyType)
        {
            if (enemyType == (int)EnemyType.Basic) return new BasicEnemy();
            if (enemyType == (int)EnemyType.Leopard) return new Leopard();
            if (enemyType == (int)EnemyType.Toad) return new Toad();
            if (enemyType == (int)EnemyType.Snail) return new Snail();
            if (enemyType == (int)EnemyType.Mouse) return new Mouse();
            if (enemyType == (int)EnemyType.Scorpion) return new Scorpion();
            return new BasicEnemy(); //enemyType wrong
        }
    }

    internal class BasicEnemy : Enemy
    {
        public BasicEnemy()
        {
            HP = maxHP = 15.0;
            attack = 5;
            speed = defaultSpeed = 1.0;
            reward = 20;

            status = new List<double>();
            for(int i=0; i<(int)StatusEffect.TotalCount; i++)
            {
                status.Add(0.0);
            }

            initHurtTimer();
        }
        public override void move(double delta_t)
        {
            defaultMove(delta_t);
        }
        public override void statusEffect(double delta_t)
        {
            defaultStatusEffect(delta_t);
        }

        public override void dealtDamage(double val)
        {
            defaultDealtDamage(val);
        }
        public override void dealtStatusEffect(StatusEffect type, double val)
        {
            defaultDealtStatusEffect(type, val);
        }
    }
}
