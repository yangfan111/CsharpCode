using UnityEngine;

namespace App.Shared.Components.Ui
{
    public class CrossHairHurtedData
    {
        private float hurtedNum;
        private float hurtedAngel;
        private Vector2 damageSrcPos;

        public CrossHairHurtedData(float _hurtedNum, float _hurtedAngel, Vector2 _damageSrcPos)
        {
            this.hurtedNum = _hurtedNum;
            this.hurtedAngel = _hurtedAngel;
            damageSrcPos = _damageSrcPos;
        }

        public float HurtedNum
        {
            get
            {
                return hurtedNum;
            }

            set
            {
                hurtedNum = value;
            }
        }

        public float HurtedAngel
        {
            get
            {
                return hurtedAngel;
            }

            set
            {
                hurtedAngel = value;
            }
        }

        public Vector2 DamageSrcPos
        {
            get
            {
                return damageSrcPos;
            }

            set
            {
                damageSrcPos = value;
            }
        }
    }
}