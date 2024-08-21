using SparkyGames.MonkeyAdventure.Common.Callbacks;
using UnityEngine;

namespace SparkyGames.MonkeyAdventure.Timers
{
    /// <summary>
    /// Timer
    /// </summary>
    public class Timer
    {
        private readonly float _delayTime;
        private readonly IActionHandler _handler;
        private float _timeDelayed = 0;
        private bool _isAlive = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="delayTime">The delay time.</param>
        /// <param name="handler">The handler.</param>
        internal Timer(float delayTime, IActionHandler handler)
        {
            _delayTime = delayTime;
            _handler = handler;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        internal void Update()
        {
            if (!_isAlive) return;
            _timeDelayed += Time.deltaTime;
            if (_timeDelayed > _delayTime)
            {
                _handler.Handler();
                _isAlive = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAlive => _isAlive;
    }
}
