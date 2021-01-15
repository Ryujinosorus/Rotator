using UnityEngine;
using UnityEditor;
namespace com.technical.test
{
    public class RotatorEditor : EditorWindow
    {
        //SelectedGO contiendra les gameObjects de notre tableau 
        public GameObject[] SelectedGO;

        //RotatorCopy stockera une copie du component Rotator, sachant que RotatorCopy[i] sera le component Rotator du gameObject SelectedGO[i]
        public RotatorProperty[] RotatorCopy;

        Vector2 scrollPosition = Vector2.zero;
        private void Awake()
        {
            //On ajoute les objects selectionne dans l'inspector
            GameObject[] tmp = new GameObject[256];
            int i = 0;
            foreach (GameObject go in Selection.gameObjects)
            {
                tmp[i++] = go;
            }
            SelectedGO = new GameObject[i];
            RotatorCopy = new RotatorProperty[256];

            for (int j = 0; j < i; j++)
            {
                SelectedGO[j] = tmp[j];
                Rotator data = tmp[j].GetComponent<Rotator>();
                if (data != null) RotatorCopy[j] = new RotatorProperty(tmp[j].GetComponent<Rotator>(), tmp[j].GetInstanceID());
                else RotatorCopy[j] = null;
            }
        }

        [MenuItem("Window/Custon/Rotators Multiple Setter")]
        public static void ShowWindow()
        {

            GetWindow<RotatorEditor>("Rotators Multiple Setter");
        }

        void OnGUI()
        {
            //Pour la responsivite
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Width(position.width), GUILayout.Height(position.height));
            //Generation du tableau dans la fenetre
            ScriptableObject scriptableObj = this;
            SerializedObject serialObj = new SerializedObject(scriptableObj);
            SerializedProperty serialProp = serialObj.FindProperty("SelectedGO");
            EditorGUILayout.PropertyField(serialProp, true);

            //True lorsque qu'on modifie le tableau
            if (serialObj.hasModifiedProperties)
            {
                serialObj.ApplyModifiedProperties();
                //On boucle sur notre nouveau tableau en modifiant RotatorCopy si il le faut
                for (int i = 0; i < SelectedGO.Length; i++)
                {
                    if (SelectedGO[i] == null) RotatorCopy[i] = null;
                    else
                    {
                        Rotator r = SelectedGO[i].GetComponent<Rotator>();
                        if (r == null) RotatorCopy[i] = null;
                        else if (RotatorCopy[i] == null) RotatorCopy[i] = new RotatorProperty(r, SelectedGO[i].GetInstanceID());
                        else if (SelectedGO[i].GetInstanceID() != RotatorCopy[i].objectID)
                            RotatorCopy[i] = new RotatorProperty(r, SelectedGO[i].GetInstanceID());
                    }
                }
            }
            reDraw();
            GUILayout.EndScrollView();

        }
        //Lance DrawSettings sur tout les object dans SelectedGO
        private void reDraw()
        {
            for (int i = 0; i < SelectedGO.Length; i++)
            {
                if (SelectedGO[i] != null) DrawSettings(i);
            }
        }

        //Affiche le component tout en permettant les modifications
        private void DrawSettings(int i)
        {
            GUILayout.Label(SelectedGO[i].name, EditorStyles.boldLabel);
            RotatorProperty rp = RotatorCopy[i];
            if (rp == null)
            {
                GUILayout.Label("Only GameObject who contain Rotator can be modify");
                return;
            }

            rp.identifier = EditorGUILayout.TextField("Id", rp.identifier);
            rp.timeBeforeStoppingInSeconds = EditorGUILayout.FloatField("Time Before Stopping (s)", rp.timeBeforeStoppingInSeconds);
            rp.shouldReverseRotation = EditorGUILayout.Toggle("Reverse rotation", rp.shouldReverseRotation);
            GUILayout.Label("Rotation settings");
            rp.rotationsSettings.ObjectToRotate = EditorGUILayout.ObjectField("Object to rotate", rp.rotationsSettings.ObjectToRotate, typeof(Transform), true) as Transform;
            rp.rotationsSettings.AngleRotation = EditorGUILayout.Vector3Field("Angle rotation", rp.rotationsSettings.AngleRotation);
            rp.rotationsSettings.TimeToRotateInSeconds = EditorGUILayout.FloatField("Time to rotate", rp.rotationsSettings.TimeToRotateInSeconds);
            if (GUILayout.Button("Save"))
            {
                Debug.Log(SelectedGO[i].name + " has been modified successfully!");
                rp.Paste(SelectedGO[i].GetComponent<Rotator>());
            }
        }
    }

    //Class permettant de stocker les données d'un component Rotator 
    public class RotatorProperty
    {
        public string identifier;
        public float timeBeforeStoppingInSeconds;
        public bool shouldReverseRotation;
        public RotationSettings rotationsSettings;
        public int objectID;

        public RotatorProperty(Rotator r, int id)
        {
            this.identifier = r.getId();
            this.timeBeforeStoppingInSeconds = r.getTimeBeforeStopping();
            this.shouldReverseRotation = r.getReverseRotation();
            this.rotationsSettings = r.getRotationSettings();
            this.objectID = id;
        }

        //Va se copier dans le component Rotator lui correspondant
        public void Paste(Rotator data)
        {
            data.setId(this.identifier);
            data.SetTimeBeforeStopping(this.timeBeforeStoppingInSeconds);
            data.SetReversePosition(this.shouldReverseRotation);
            data.SetRotationSettings(this.rotationsSettings);
        }
    }
}