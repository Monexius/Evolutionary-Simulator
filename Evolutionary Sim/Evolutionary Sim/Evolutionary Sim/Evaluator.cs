using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolutionary_Sim
{
    public class Evaluator : IPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;
        private bool _stopConditionSatisfied;
        public int oldFruits;
        public double fitness = 0;
        HealthBar health;
        Agent neatPlayer;
        Game1 game;

        public double getScore(int numOfFruits)
        {
            if (numOfFruits > oldFruits)
                return 0.0010;
            if (numOfFruits == oldFruits)
                return 0.001;

            return 0;
        }

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        //Called at the end of every death
        public FitnessInfo Evaluate(IBlackBox box)
        {
            

            if (HealthBar.GetHealth() < 1) // If agent died 
            {
                neatPlayer = new Agent();
                //neatPlayer.Iterate();
                neatPlayer.InitializeBrain(box);

                int timeSurvived = neatPlayer.GetTimeSurvived();
                int fruits = neatPlayer.GetFruitTotal();
                fitness += getScore(fruits); // get fitness
                //Debug.WriteLine(fitness);
                oldFruits = fruits;
            }

            // Update the evaluation counter.
            _evalCount++;
            //Debug.WriteLine("Eval counter: " + _evalCount);

            if (fitness >= 1002)
                _stopConditionSatisfied = true;

            // Return the fitness score
            return new FitnessInfo(fitness, fitness);

        }

        public void Reset()
        {

        }

    }
}
