﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Collections;
using BulletHell.Physics;
using BulletHell.Gfx;

namespace BulletHell.GameLib.EntityLib
{
    public interface EntityManager
    {
        void Add(Entity e);
        void RemovePermanently(Entity e);
        void Remove(Entity e);
        IEnumerable<Entity> Entities(double t);
        IEnumerable<Entity> BulletShooters(double Time);

        bool Contains(Entity entity);
    }
    public class AdvancedEntityManager : EntityManager
    {
        private LookupLinkedListSet<Entity> entities;
        private LookupLinkedListSet<Entity> bulletShooters;
        private PhysicsManager pm;
        private RenderManager rm;

        private double oldTime;

        public AdvancedEntityManager(PhysicsManager phys, RenderManager renders,params double[] ints)
        {
            pm = phys;
            rm = renders;
            oldTime = 0;
            entities = new LookupLinkedListSet<Entity>();
            bulletShooters = new LookupLinkedListSet<Entity>();
        }

        public void Add(Entity e)
        {
            if (e.Manager == this)
            {
                return;
            }
            e.Manager = this;
            ProcessAddition(e);
        }
        private void ProcessAddition(Entity e)
        {
            pm.Add(e);
            rm.Add(e);
            entities.Add(e);
            if (e.Emitter != null)
            {
                bulletShooters.Add(e);
            }
        }
        private void ProcessRemoval(Entity e)
        {
            pm.Remove(e);
            rm.Remove(e);
            entities.Remove(e);
            if (e.Emitter != null)
                bulletShooters.Remove(e);
        }

        public void Remove(Entity e)
        {
            if (e.Manager != this)
                return;
            e.Manager = null;
            ProcessRemoval(e);
        }

        public IEnumerable<Entity> Entities(double t)
        {
            return entities;
        }
        public IEnumerable<Entity> BulletShooters(double t)
        {
            return bulletShooters;
        }


        public void RemovePermanently(Entity e)
        {
            if (e.Manager != this)
                return;
            e.Manager = null;
            if (e.CreationTime > oldTime || (e.InvisibilityTime != -1 && e.InvisibilityTime < oldTime))
            {
                ProcessPermanentRemoval(e);
            }
        }

        private void ProcessPermanentRemoval(Entity e)
        {
            pm.RemovePermanently(e);
            rm.RemovePermanently(e);
            entities.RemovePermanently(e);
            if (e.Emitter != null)
                bulletShooters.RemovePermanently(e);
        }


        public bool Contains(Entity entity)
        {
            return entities.Contains(entity);
        }
    }
}
