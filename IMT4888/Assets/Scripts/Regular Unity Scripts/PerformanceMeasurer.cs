using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class PerformanceMeasurer : MonoBehaviour {
    public float _timeSpentMeasuring = 30f;
    public string _resultsFileName = "";

    private List<float> _frameLatencies = new List<float>();
    private float _measurementTimer = 0;
    private bool _isMeasuring = false;

	void Update () {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _isMeasuring = true;
        }

        if(_isMeasuring)
        {
            _frameLatencies.Add(Time.deltaTime);
            _measurementTimer += Time.deltaTime;

            if(_measurementTimer >= _timeSpentMeasuring)
            {
                _measurementTimer = 0;
                _isMeasuring = false;
                WriteResultsToFile();
                _frameLatencies.Clear();
                Debug.Log("File written!");
            }
        }
	}

    // Based on https://stackoverflow.com/questions/18757097/writing-data-into-csv-file-in-c-sharp
    void WriteResultsToFile()
    {
        using (var w = new StreamWriter(System.IO.Path.GetDirectoryName(Application.dataPath) + "/Assets/TestResults/" + _resultsFileName))
        {
            var column1 = "CapturedFrameNumber";
            var column2 = "LatencyInMs";
            var line = string.Format("{0},{1}", column1, column2);
            w.WriteLine(line);
            w.Flush();

            for (int i = 0; i < _frameLatencies.Count; i++)
            {
                column1 = i.ToString();
                column2 = (_frameLatencies[i] * 1000).ToString(CultureInfo.InvariantCulture);
                line = string.Format("{0},{1}", column1, column2);
                w.WriteLine(line);
                w.Flush();
            }
        }
    }
}
