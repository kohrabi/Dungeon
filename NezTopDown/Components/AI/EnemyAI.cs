using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.AI
{
    public class EnemyAI : Component, Nez.IUpdateable
    {
        private List<SteeringBehaviour> steeringBehaviours;
        private List<Detector> detectors;
        private AIData aiData;

        private const float detectionDelay = 0.05f, aiUpdateDelay = 0.06f, attackDelay = 1f;

        private float attackDistance = 0.5f;
        /*
        //Inputs sent from the Enemy AI to the Enemy controller
        public UnityEvent OnAttackPressed;
        public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
        */
        private Vector2 movementInput;

        private ContextSolver movementDirectionSolver;

        bool following = false;

        public EnemyAI()
        {
            detectors = new List<Detector>();
            steeringBehaviours = new List<SteeringBehaviour>();
        }

        public override void OnAddedToEntity()
        {
            aiData = Entity.GetComponent<AIData>();
            movementDirectionSolver = Entity.GetComponent<ContextSolver>();

            base.OnAddedToEntity();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            aiData.currentTarget = null;
            steeringBehaviours.Clear();
            detectors.Clear();
        }

        private void PerformDetection()
        {
            foreach (Detector detector in detectors)
            {
                detector.Detect(aiData);
            }
        }

        public void AddDetector(Detector detector)
        {
            Debug.ErrorIf(detector == null, "Null Detector! this might never happens");
            detectors.Add(detector);
        }

        public void AddSteeringBehaviour(SteeringBehaviour seekBehaviour)
        {
            Debug.ErrorIf(seekBehaviour == null, "Null Behaviour! this might never happens");
            steeringBehaviours.Add(seekBehaviour);
        }


        float _remainingDelay = detectionDelay;
        void Nez.IUpdateable.Update()
        {
            _remainingDelay -= Time.DeltaTime;
            if (_remainingDelay <= 0)
            {
                PerformDetection();
                _remainingDelay = detectionDelay;
            }

            //Enemy AI movement based on Target availability
            if (aiData.currentTarget != null)
            {
                //Looking at the Target
                //OnPointerInput?.Invoke(aiData.currentTarget.position);
                if (following == false)
                {
                    following = true;
                    Core.StartCoroutine(ChaseAndAttack());
                }
            }
            else if (aiData.GetTargetsCount() > 0)
            {
                //Target acquisition logic
                aiData.currentTarget = aiData.targets[0];
            }
            //Moving the Agent
            Entity.GetComponent<Enemy>().MovementInput = movementInput;
        }

        private IEnumerator ChaseAndAttack()
        {
            if (aiData.currentTarget == null)
            {
                //Stopping Logic
                Console.WriteLine("Stopping");
                movementInput = Vector2.Zero;
                following = false;
                yield break;
            }
            else
            {
                float distance = Vector2.Distance(aiData.currentTarget.Position, Entity.Transform.Position);

                if (distance < attackDistance)
                {
                    //Attack logic
                    movementInput = Vector2.Zero;
                    //OnAttackPressed?.Invoke();
                    yield return Coroutine.WaitForSeconds(attackDelay);
                    yield return Core.StartCoroutine(ChaseAndAttack());
                }
                else
                {
                    //Chase logic
                    movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                    yield return Coroutine.WaitForSeconds(aiUpdateDelay);
                    yield return Core.StartCoroutine(ChaseAndAttack());
                }

            }
        }
    }

}
