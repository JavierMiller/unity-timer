using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SparkyGames.Core.Timers
{
    /// <summary>
    /// Timer Manager
    /// </summary>
    public class TimerManager
    {
        private IList<Timer> _timerCollection = new List<Timer>();

        private static TimerBehaviourHook _timerBehaivourHook;
        private static TimerManager _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static TimerManager Instance => _instance ?? (_instance = new TimerManager());

        /// <summary>
        /// Starts the specified delay time.
        /// </summary>
        /// <param name="delayTime">The delay time.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public static Timer Start(float delayTime, Action handler) =>
            Instance.Create(delayTime, handler);

        /// <summary>
        /// Starts the asynchronous.
        /// </summary>
        /// <param name="delayTime">The delay time.</param>
        public static async Task StartAsync(float delayTime)
        {
            var isFinished = false;
            Instance.Create(delayTime, new Action(() => isFinished = true));

            while (!isFinished)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Creates the specified delay time.
        /// </summary>
        /// <param name="delayTime">The delay time.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        private Timer Create(float delayTime, Action handler)
        {
            if (_timerBehaivourHook == null)
            {
                _timerBehaivourHook = UnityEngine.Object.FindObjectOfType<TimerBehaviourHook>();
                if (_timerBehaivourHook == null)
                {
                    var gameObject = new GameObject("TimerBehaviour", typeof(TimerBehaviourHook));
                    _timerBehaivourHook = gameObject.AddComponent<TimerBehaviourHook>();
                    _timerBehaivourHook.OnUpdate += _timerBehaivourHook_OnUpdate;
                    _timerBehaivourHook.OnDestroyed += _timerBehaivourHook_OnDestroyed;
                }
            }

            var timer = new Timer(delayTime, handler);
            _timerCollection.Add(timer);
            _timerBehaivourHook.enabled = true;

            return timer;
        }

        /// <summary>
        /// Handles the OnDestroyed event of the _timerBehaivourHook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void _timerBehaivourHook_OnDestroyed(object sender, EventArgs e)
        {
            _timerBehaivourHook.OnUpdate -= _timerBehaivourHook_OnUpdate;
            _timerBehaivourHook.OnDestroyed -= _timerBehaivourHook_OnDestroyed;
            _timerBehaivourHook = null;
            _timerCollection.Clear();
        }

        /// <summary>
        /// Handles the OnUpdate event of the _timerBehaivourHook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void _timerBehaivourHook_OnUpdate(object sender, EventArgs e)
        {
            for (var i = _timerCollection.Count - 1; i >= 0; i--)
            {
                var timer = _timerCollection[i];
                timer.Update();
                if (!timer.IsAlive) _timerCollection.Remove(timer);
            }

            if (!_timerCollection.Any()) _timerBehaivourHook.enabled = false;
        }

        /// <summary>
        /// Timer Behaviour Hook
        /// </summary>
        /// <seealso cref="MonoBehaviour" />
        private class TimerBehaviourHook : MonoBehaviour
        {
            /// <summary>
            /// Occurs when [on update].
            /// </summary>
            public event EventHandler OnUpdate;
            /// <summary>
            /// Occurs when [on destroyed].
            /// </summary>
            public event EventHandler OnDestroyed;

            /// <summary>
            /// Updates this instance.
            /// </summary>
            private void Update() => OnUpdate?.Invoke(this, EventArgs.Empty);

            /// <summary>
            /// Called when [destroy].
            /// </summary>
            private void OnDestroy() => OnDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }
}
