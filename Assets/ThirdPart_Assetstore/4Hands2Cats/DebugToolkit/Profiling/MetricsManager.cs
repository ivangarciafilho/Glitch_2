using DebugToolkit.Interaction.Commands;
using UnityEngine;

namespace DebugToolkit.Profiling
{
    public class MetricsManager : MonoBehaviour
    {
        [Header("Command")]
        [SerializeField] BooleanCommand metricCommand;

        [Header("Params")]
        [SerializeField] GameObject metricPrefab;
        private GameObject metric;

        private void Awake()
        {
            metricCommand.OnIsValid += MetricCommand_OnIsValid;
        }

        private void OnDestroy()
        {
            metricCommand.OnIsValid -= MetricCommand_OnIsValid;
        }

        private void MetricCommand_OnIsValid(bool obj)
        {
            if (this == null) return;

            if (obj)
            {
                InstantiateMetric();
            }
            else
            {
                metric.GetComponent<Metrics>().Dispose();
            }
        }

        private void InstantiateMetric()
        {
            if (metric == null)
            {
                metric = Instantiate(metricPrefab);
            }
        }
    }
}

