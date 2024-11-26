using UnityEngine;

namespace InTerra
{
    [AddComponentMenu("InTerra/InTerra Tracks")]
    public class InTerra_Track : MonoBehaviour
    {
        [SerializeField] public Material trackMaterial;

        [SerializeField] [Min(0.01f)] public float quadWidth = 0.45f;
        [SerializeField] [Min(0.01f)] public float quadLenght = 1.0f;
        [SerializeField] public float quadOffsetX = 0.0f;
        [SerializeField] public float quadOffsetZ = 0.0f;

        [SerializeField] float stepSize = 0.05f;
        [SerializeField] float lenghtUV = 3f;

        [SerializeField] [Min(0)] public float groundedCheckDistance = 0.6f;
        [SerializeField] public float startCheckDistance = 0.0f;
        [SerializeField] [Min(0)] float time = 0.1f;
        [SerializeField] [Min(25)] public float ereaseDistance = 75.0f;

        [SerializeField] public bool delete;

        private Vector3 lastPosition;
        private Vector3 lastVertexUp;
        private Vector3 lastVertexDown;
        
        private float lastUV0_X;
        private float lastUV1_X;

        private float lastVertCreationTime;

        bool grounded;
        bool lastGrounded;

        public float targetTime = 0;
        float groupSize = 0.5f;
              
        Vector3 groupLastPosition;

        bool wheelTrack;
        bool defaultTrack;

        bool initTrack;
        bool initTime;

        int c = 0;
        [SerializeField, HideInInspector] GameObject trackFadeOut;
        [SerializeField, HideInInspector] GameObject tracks;

        GameObject TrackMesh;

        private void Update()
        {
            if (trackMaterial != null)
            {
                if (trackMaterial.IsKeywordEnabled("_TRACKS"))
                {
                    wheelTrack = true;
                }
                else if (!trackMaterial.IsKeywordEnabled("_FOOTPRINTS"))
                {
                    defaultTrack = true;
                }
                if (InTerra_Data.TracksFadingEmabled())
                {
                    trackMaterial.SetFloat("_TrackFadeTime", InTerra_Data.GetUpdaterScript().TracksFadingTime);
                    trackMaterial.SetFloat("_TrackTime", Time.timeSinceLevelLoad);
                }          
            }           

            RaycastHit hit;
            bool meshTerrainHit = false;
            bool terrainHit = false;
            bool integratedObjectHit = false;

            grounded = false;

            Vector3 forwardVector = GetForwardVector();
            if(Physics.Raycast(transform.position - new Vector3(0, -startCheckDistance,0), Vector3.down, out hit, groundedCheckDistance) )
            {
                if (hit.collider.GetComponent<Terrain>() && InTerra_Data.CheckTerrainShader(hit.collider.GetComponent<Terrain>().materialTemplate))
                {
                    terrainHit = true;
                }
                else if (hit.collider.GetComponent<Renderer>() && InTerra_Data.CheckMeshTerrainShader(hit.collider.GetComponent<Renderer>().sharedMaterial))
                {
                    meshTerrainHit = true;
                }
                else if (hit.collider.GetComponent<Renderer>() && InTerra_Data.CheckMeshTerrainShader(hit.collider.GetComponent<Renderer>().sharedMaterial))
                {
                    integratedObjectHit = true;
                }
                if(terrainHit || meshTerrainHit || integratedObjectHit)
                {
                    grounded = true;
                }
                else
                {
                    initTrack = false;
                }
            }

            if ((!initTrack ) && grounded)
            {
                lastPosition = new Vector3(transform.position.x, 0, transform.position.z) - forwardVector * quadLenght;
                lastVertexUp = VertexPositions()[0] - forwardVector * quadLenght;
                lastVertexDown = VertexPositions()[1] - forwardVector * quadLenght;
                lastVertCreationTime = Time.timeSinceLevelLoad;

                CreateTrackMesh(0);

                lastPosition = new Vector3(transform.position.x, 0, transform.position.z);
                groupLastPosition = new Vector3(transform.position.x, 0, transform.position.z);

                if (wheelTrack)
                {            
                    CreateTrackMesh(2);                   
                }

                initTrack = true;
                if(!initTime && trackMaterial)
                {
                    trackMaterial.SetFloat("_TrackTime", Time.timeSinceLevelLoad);
                    initTime = true;
                }            
            }

            else
            {
                float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), lastPosition);
                if (wheelTrack)
                {
                    if (distance > stepSize && lastGrounded)
                    {
                        if (wheelTrack && tracks.transform.childCount == 0)
                        {
                            lastPosition = new Vector3(transform.position.x, 0, transform.position.z) - forwardVector * quadLenght;
                            lastVertexUp = VertexPositions()[0] - forwardVector * quadLenght;
                            lastVertexDown = VertexPositions()[1] - forwardVector * quadLenght;
                            lastVertCreationTime = Time.timeSinceLevelLoad;

                            CreateTrackMesh(0);

                            lastPosition = new Vector3(transform.position.x, 0, transform.position.z);
                            groupLastPosition = new Vector3(transform.position.x, 0, transform.position.z);
                        }

                        CreateTrackMesh(1);                       
                        CreateTrackMesh(2);                       
                        lastPosition = new Vector3(transform.position.x, 0, transform.position.z);
                    }
                }
                else
                { 
                    if (distance > stepSize && grounded && (targetTime <= 0.0f))
                    {
                        CreateTrackMesh(0);
                        lastPosition = new Vector3(transform.position.x, 0, transform.position.z);
                        targetTime = time;
                    }
                }

                if (grounded)
                {
                    targetTime -= Time.deltaTime;  
                }
                else
                {
                    targetTime = time;
                }
            }
            lastGrounded = grounded;

            if (InTerra_Data.TracksFadingEmabled())
            {
                if (tracks && tracks.transform.childCount > 0)
                {
                    //Delte invisible stamps
                    Mesh oldestStamp = tracks.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
                    Vector2 vertexTime = oldestStamp.uv3[oldestStamp.uv3.Length - 3];
                    if ((Time.timeSinceLevelLoad - vertexTime.y) > InTerra_Data.GetUpdaterScript().TracksFadingTime)
                    {
                        Destroy(tracks.transform.GetChild(0).gameObject);
                    } 
                }

                //Prevent the last created stamp to fade
                if (grounded && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), lastPosition) < stepSize && !wheelTrack)
                {
                    Mesh lastCreatedStamp = tracks.transform.GetChild(tracks.transform.childCount - (1)).GetComponent<MeshFilter>().mesh;
                    int vertLenth = lastCreatedStamp.uv3.Length;

                    Vector2[] timeUpdate = new Vector2[vertLenth];
                    lastCreatedStamp.uv3.CopyTo(timeUpdate, 0);
                    timeUpdate[vertLenth - 1].y = Time.timeSinceLevelLoad;
                    timeUpdate[vertLenth - 2].y = Time.timeSinceLevelLoad;
                    timeUpdate[vertLenth - 3].y = Time.timeSinceLevelLoad;
                    timeUpdate[vertLenth - 4].y = Time.timeSinceLevelLoad;
                    
                    lastCreatedStamp.uv3 = timeUpdate;
                }
            }
        }

        public void CreateTrackMesh(int positionIndex)
        {
            var dataScript = InTerra_Data.GetUpdaterScript();
            bool newObject = groupSize < Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), groupLastPosition);           
            if (tracks == null)
            {
                tracks = new GameObject("InTerra_Tracks_" + this.name);
            }

            Mesh tMesh;

            if (positionIndex == 2)
            {
                if(lastGrounded)
                {
                    DestroyImmediate(trackFadeOut);
                }
                trackFadeOut = new GameObject("Track Fade out Stamp " + c);
                trackFadeOut.AddComponent<MeshFilter>();
                trackFadeOut.AddComponent<MeshRenderer>();
                trackFadeOut.transform.parent = tracks.transform;
                tMesh = trackFadeOut.GetComponent<MeshFilter>().mesh;
            }
            else
            {
                if (TrackMesh == null || newObject)
                {
                    c += 1;
                    TrackMesh = new GameObject("Track Stamp " + c);
                    TrackMesh.AddComponent<MeshFilter>();
                    TrackMesh.AddComponent<MeshRenderer>();                   
                }
                tMesh = TrackMesh.GetComponent<MeshFilter>().mesh;
            }

            TrackMesh.transform.parent = tracks.transform;

            int vertLenght;
            int trianglesLenght;

            if (!newObject)
            {            
                vertLenght = tMesh.vertices.Length + 4;
                trianglesLenght = tMesh.triangles.Length + 6;
            }
            else
            {
                tMesh = new Mesh();               
                vertLenght = 4;
                trianglesLenght = 6;
                groupLastPosition = new Vector3(transform.position.x, 0, transform.position.z);
            }

            tMesh.name = "Track Mesh " + c;
            Vector3[] vertices = new Vector3[vertLenght];
            Vector2[] uv = new Vector2[vertLenght];
            Vector2[] uv2 = new Vector2[vertLenght];
            Vector2[] uv3 = new Vector2[vertLenght];

            int[] triangles = new int[trianglesLenght];

            Vector3 forwardVector = GetForwardVector();

            float distance;
            Vector3 newVertexUp;
            Vector3 newVertexDown;

            if (!newObject)
            {
                tMesh.vertices.CopyTo(vertices, 0);
                tMesh.uv.CopyTo(uv, 0);
                tMesh.uv2.CopyTo(uv2, 0);
                tMesh.uv3.CopyTo(uv3, 0);
                tMesh.triangles.CopyTo(triangles, 0);
            }
     
            int vIndex = vertices.Length - 4;

            int vIndex0 = vIndex + 0;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            if (positionIndex == 0)
            {
                if (wheelTrack)
                {
                    newVertexUp = VertexPositions()[0];
                    newVertexDown = VertexPositions()[1];
                    SetFadingIn(vIndex, ref uv3);
                    lastVertCreationTime = Time.timeSinceLevelLoad;
                }
                else
                {
                    lastVertexUp = VertexPositions()[0] - forwardVector * (quadLenght / 2);
                    lastVertexDown = VertexPositions()[1] - forwardVector * (quadLenght / 2);
                    newVertexUp = VertexPositions()[0] + forwardVector * (quadLenght / 2);
                    newVertexDown = VertexPositions()[1] + forwardVector * (quadLenght / 2);
                    SetDefaultFading(Time.timeSinceLevelLoad, vIndex, ref uv3);
                }               

                distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), lastPosition);
            }
            else
            {
                newVertexUp = VertexPositions()[0];
                newVertexDown = VertexPositions()[1];
                distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), lastPosition);
                float firstTwoVertsTime = wheelTrack ? lastVertCreationTime : Time.timeSinceLevelLoad;
                SetDefaultFading(firstTwoVertsTime, vIndex, ref uv3);

            }
            if (positionIndex == 2)
            {
                newVertexUp = VertexPositions()[0] + forwardVector * quadLenght;
                newVertexDown = VertexPositions()[1] + forwardVector * quadLenght;
                distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z) + forwardVector * quadLenght, lastPosition);
                SetFadingOut(vIndex, ref uv3);
            }
            else
            {
                dataScript.TrackDepthIndex += 0.0001f;
            }

            vertices[vIndex0] = lastVertexDown;
            vertices[vIndex1] = lastVertexUp;

            lastUV0_X = lastUV0_X % 1;
            lastUV1_X = lastUV1_X % 1;

            vertices[vIndex2] = newVertexUp;
            vertices[vIndex3] = newVertexDown;

            if (wheelTrack)
            {
                uv[vIndex0] = new Vector2(lastUV0_X, 1);
                uv[vIndex1] = new Vector2(lastUV1_X, 0);
                uv[vIndex2] = new Vector2((lastUV0_X + (1 / lenghtUV * distance)), 0);
                uv[vIndex3] = new Vector2((lastUV1_X + (1 / lenghtUV * distance)), 1);
            }
            else
            {
                uv[vIndex0] = new Vector2(1, 1);
                uv[vIndex1] = new Vector2(1, 0);
                uv[vIndex2] = new Vector2(0, 0);
                uv[vIndex3] = new Vector2(0, 1);
            }

            int tIndex = triangles.Length - 6;
            triangles[tIndex + 2] = vIndex0;
            triangles[tIndex + 1] = vIndex1;
            triangles[tIndex + 0] = vIndex2;

            triangles[tIndex + 5] = vIndex0;
            triangles[tIndex + 4] = vIndex2;
            triangles[tIndex + 3] = vIndex3;

            tMesh.vertices = vertices;
            tMesh.uv = uv;
            tMesh.uv2 = uv;
            tMesh.uv3 = uv3;

            tMesh.triangles = triangles;

            lastVertCreationTime = Time.timeSinceLevelLoad;

            if (positionIndex != 2)
            {
                lastUV0_X = uv[vIndex2].x;
                lastUV1_X = uv[vIndex3].x;
                lastVertexUp = newVertexUp;
                lastVertexDown = newVertexDown;
            }

            if (positionIndex == 2)
            {
                trackFadeOut.GetComponent<MeshFilter>().mesh = tMesh;
                trackFadeOut.GetComponent<MeshRenderer>().sharedMaterial = trackMaterial;
                trackFadeOut.layer = InTerra_Data.GetGlobalData().trackLayer;
            }
            else
            {
                if (!TrackMesh.TryGetComponent<MeshFilter>(out MeshFilter mr))
                {
                    TrackMesh.AddComponent<MeshRenderer>();
                    TrackMesh.AddComponent<MeshFilter>();
                }

                TrackMesh.layer = InTerra_Data.GetGlobalData().trackLayer;
               
                TrackMesh.GetComponent<MeshFilter>().mesh = tMesh;

                if (newObject || TrackMesh.GetComponent<MeshRenderer>().sharedMaterial == null)
                {
                    TrackMesh.GetComponent<MeshRenderer>().sharedMaterial = trackMaterial;
                    TrackMesh.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off ;
                    TrackMesh.GetComponent<MeshRenderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    TrackMesh.GetComponent<MeshRenderer>().receiveShadows = false;
                }
            }

            if(tracks.transform.childCount != 0)
            { 
                for (int i = 0; i < tracks.transform.childCount; i++)
                {
                    Mesh m = tracks.transform.GetChild(i).GetComponent<MeshFilter>().mesh;

                    if (Vector2.Distance(new Vector2(m.vertices[0].x, m.vertices[0].z), new Vector2(transform.position.x, transform.position.z)) > ereaseDistance)
                    {
                        Destroy(tracks.transform.GetChild(i).gameObject);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        public Vector3 GetForwardVector()
        {
            if(initTrack && defaultTrack)
            {
                return (new Vector3(transform.position.x, 0, transform.position.z) - lastPosition).normalized;
            }
            else
            {
                return Vector3.Cross((transform.right).normalized, new Vector3(0, 1f, 0));
            } 
        }

        Vector3[] VertexPositions()
        {
            Vector3[] vecPos = new Vector3[2];
            Vector3 normal2D = new Vector3(0, 1f, 0);
            Vector3 offsetPossition = OffsetedStampPosition();

            Vector3 position = new Vector3(offsetPossition.x, InTerra_Data.TracksStampYPosition(), offsetPossition.z);

            vecPos[0] = position + Vector3.Cross(GetForwardVector(), normal2D) * (quadWidth / 2);
            vecPos[1] = position + Vector3.Cross(GetForwardVector(), normal2D * -1f) * (quadWidth / 2);

            return vecPos;
        }

        public Vector3[] VertexDebugPositions()
        {
            Vector3[] vecPos = VertexPositions();
            Vector3[] debugVecPosition = new Vector3[2];

            debugVecPosition[0] = new Vector3(vecPos[0].x, transform.position.y, vecPos[0].z);
            debugVecPosition[1] = new Vector3(vecPos[1].x, transform.position.y, vecPos[1].z);

            return debugVecPosition;
        }

        private Vector3 OffsetedStampPosition()
        {
            Vector3 offsetedPos = transform.position + Vector3.Cross(GetForwardVector(), new Vector3(0, 1f, 0)) * (quadOffsetX);
            offsetedPos += GetForwardVector() * (quadOffsetZ);
            return offsetedPos;
        }

        void SetFadingIn(int vertLenght, ref Vector2[] uv3)
        {
            uv3[vertLenght + 0] = new Vector2(0, lastVertCreationTime);
            uv3[vertLenght + 1] = new Vector2(0, lastVertCreationTime);
            uv3[vertLenght + 2] = new Vector2(1, Time.timeSinceLevelLoad);
            uv3[vertLenght + 3] = new Vector2(1, Time.timeSinceLevelLoad);
        }

        void SetFadingOut(int vertLenght, ref Vector2[] uv3)
        {
            uv3[vertLenght + 0] = new Vector2(1, Time.timeSinceLevelLoad);
            uv3[vertLenght + 1] = new Vector2(1, Time.timeSinceLevelLoad);
            uv3[vertLenght + 2] = new Vector2(0, Time.timeSinceLevelLoad);
            uv3[vertLenght + 3] = new Vector2(0, Time.timeSinceLevelLoad);
        }

        void SetDefaultFading(float firstTwoVertsTime, int vertLenght, ref Vector2[] uv3)
        {
            uv3[vertLenght + 0] = new Vector2(1, firstTwoVertsTime);
            uv3[vertLenght + 1] = new Vector2(1, firstTwoVertsTime);
            uv3[vertLenght + 2] = new Vector2(1, Time.timeSinceLevelLoad);
            uv3[vertLenght + 3] = new Vector2(1, Time.timeSinceLevelLoad);
        }
    }
}
