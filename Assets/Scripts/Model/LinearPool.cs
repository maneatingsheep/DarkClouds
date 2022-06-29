using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class LinearPool<T> where T : BaseEnemyView {

        private int _firstAvalible = 0;

        private List<T> _inventory = new List<T>();


        public T GetFromPool()  {

            if (_firstAvalible == _inventory.Count) {
                return null;
            }
            
            var result = _inventory[_firstAvalible];
            _firstAvalible++;

            return result;
        }

        internal void GiveToPool(T obj) {
            
            //get index first
            int index = _inventory.IndexOf(obj);

            _inventory.Add(obj);
            if (index > -1) {
                _inventory.RemoveAt(index);
                _firstAvalible--;
            }
        }
    }
}