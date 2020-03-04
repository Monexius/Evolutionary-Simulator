using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
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
        public int oldTime;

        HealthBar health;
        Agent neatPlayer;
        Game1 game;

        public int getScore(int numOfFruits, int timeSurvived)
        {
            if (numOfFruits > oldFruits && timeSurvived > oldTime)
                return 10;

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
            double fitness = 0;
            neatPlayer = new Agent();
            neatPlayer.Iterate();
            neatPlayer.InitializeBrain(box);

            if(HealthBar.GetHealth() < 1) // If agent died 
            {
                int timeSurvived = neatPlayer.GetTimeSurvived();
                int fruits = neatPlayer.GetFruitTotal();
                fitness += getScore(fruits, timeSurvived); // get fitness

                oldFruits = fruits;
                oldTime = timeSurvived;
            }

            // Update the evaluation counter.
            _evalCount++;

            // If the network plays perfectly, it will beat the random player
            // and draw the optimal player.
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
