using UnityEngine;
using UnityEditor;
using System.Collections;
namespace com.technical.test
{
    /// <summary>
    ///     Rotates a transform with a settable angular speed 
    ///     for a specific duration
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        private string _identifier = "The Amazing Rotator";
        [Header("Rotation parameters")]
        [SerializeField]
        private float _timeBeforeStoppingInSeconds = 3;
        [SerializeField]
        private bool _shouldReverseRotation = false;

        [SerializeField]
        private RotationSettings _rotationsSettings = default;

        private float _angleModifier = 1f;
        private float _timePassed;

        void Awake()
        {
            CalculateModifierAccordingToReverse();
        }

        private void CalculateModifierAccordingToReverse()
        {
            _angleModifier = 1f;
            if (_shouldReverseRotation)
            {
                _angleModifier = -1f;
            }
        }

        void OnEnable()
        {
            _timePassed = 0f;
        }

        void Update()
        {
            Vector3 angularSpeedPerSeconds = _rotationsSettings.AngleRotation / _rotationsSettings.TimeToRotateInSeconds;
            _rotationsSettings.ObjectToRotate.rotation *= Quaternion.Euler(_angleModifier * Time.deltaTime * angularSpeedPerSeconds);
            _timePassed += Time.deltaTime;
            if (_timePassed > _timeBeforeStoppingInSeconds)
            {
                this.enabled = false;
            }
        }

        void OnValidate()
        {
            CalculateModifierAccordingToReverse();
        }

        public override string ToString()
        {
            return $"{_identifier} makes a rotation with an angle of {_rotationsSettings.AngleRotation} every {_rotationsSettings.TimeToRotateInSeconds} seconds";
        }

        //Tout simplement des getteurs et des setteurs pour avoir acces et pouvoir modifier les attributs
        public string getId()
        {
            return _identifier;
        }
        public void setId(string id)
        {
            this._identifier = id;
        }
        public float getTimeBeforeStopping()
        {
            return _timeBeforeStoppingInSeconds;
        }
        public void SetTimeBeforeStopping(float f)
        {
            this._timeBeforeStoppingInSeconds = f;
        }
        public bool getReverseRotation()
        {
            return _shouldReverseRotation;
        }
        public void SetReversePosition(bool b)
        {
            this._shouldReverseRotation = b;
        }
        public RotationSettings getRotationSettings()
        {
            return _rotationsSettings;
        }
        public void SetRotationSettings(RotationSettings rs)
        {
            this._rotationsSettings = rs;
        }
    }


    //Pour avoir le bouton dans l'inspector qui ouvrira notre fenetre
    [CustomEditor(typeof(Rotator))]
    public class RotatorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Open Rotator panel"))
            {
                RotatorEditor window = (RotatorEditor)EditorWindow.GetWindow(typeof(RotatorEditor), false, "Rotators Multiple Setter");
                window.Show();
            }
        }
    }
}