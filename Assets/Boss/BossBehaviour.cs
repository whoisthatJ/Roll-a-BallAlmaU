using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [Header("Place your script that controls the main Camera here...")]
    public MonoBehaviour _cameraFollowScript;
    public float _cameraBackoffZ = 95f;
    public float _cameraBackoffY = 45f;
    [Header ("Lasers")]
    public float _laserSpeed = 5f;
    public float _eyeGlowTime = 2.2f;
    public float _shootTime = 3f;
    public float _cooldownTime = 5f;
    public LineRenderer _leftLaserLR;
    public LineRenderer _rightLaserLR;
    public Light _leftLight;
    public Light _rightLight;
    public Transform _leftEye;
    public Transform _rightEye;
    public Transform _player;
    public Rigidbody _damageDealerRB;
    public LaserDamageDealer _damageDealer;

    [Header("Audio")]
    public AudioSource _laserAudioSource;
    public AudioSource _bossAudioSource;
    public AudioSource _bgMusicAudioSource;
    public AudioClip _movingAClip;
    public AudioClip _voiceAClip;
    public Transform[] _platforms;
    public Transform _enterPosTransform;
    public Transform _focusPosTransform;


    private bool glowEyes;
    private bool shoot;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        //_damageDealerRB.position = _player.position;
        _leftLaserLR.SetPosition(0, _leftEye.position);
        _rightLaserLR.SetPosition(0, _rightEye.position);
        _leftLaserLR.SetPosition(1, _leftEye.position);
        _rightLaserLR.SetPosition(1, _rightEye.position);        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GlowEyes();
        Shoot();
    }

    public void StartBossFight()
    {
        gameObject.SetActive(true);
        _bgMusicAudioSource.Play(); 
        StartCoroutine(PlatformsFalling());
        StartCoroutine(BossEnter());
    }
    IEnumerator BossEnter()
    {
        _bossAudioSource.clip = _movingAClip;
        _bossAudioSource.Play();
        Vector3 enterPos = _enterPosTransform.position;
        StartCoroutine(CameraShake(_movingAClip.length, 0.5f));
        float moveSpeed = Vector3.Distance(transform.position, enterPos)/_movingAClip.length;
        while (transform.position != enterPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, enterPos, Time.deltaTime * moveSpeed);
            yield return null;
        }
        _bossAudioSource.clip = _voiceAClip;
        _bossAudioSource.Play();
        yield return new WaitForSeconds(_voiceAClip.length);
        StartCoroutine(ShootLasers());
    }
    IEnumerator CameraShake(float duration, float magnitude)
    {
        //_cameraFollowScript.transform.eulerAngles = new Vector3(_cameraFollowScript.transform.eulerAngles.x, -90f, _cameraFollowScript.transform.eulerAngles.z);

        _cameraFollowScript.enabled = false;
        _cameraFollowScript.transform.position = transform.position + transform.forward * _cameraBackoffZ + transform.up * _cameraBackoffY;
        
        Vector3 originalPos = _cameraFollowScript.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;
            _cameraFollowScript.transform.position = originalPos + new Vector3(x, y, z);
            _cameraFollowScript.transform.LookAt(_focusPosTransform.position);
            if (_cameraFollowScript.transform.childCount > 0)
            {
                _cameraFollowScript.transform.eulerAngles -= Camera.main.transform.localEulerAngles;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        _cameraFollowScript.transform.position = originalPos;
        _cameraFollowScript.enabled = true;
    }
    IEnumerator PlatformsFalling()
    {
        foreach (Transform platform in _platforms)
        {
            foreach (Transform t in platform)
            {
                if(t.name == "Floor")
                {
                    t.gameObject.AddComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                    t.gameObject.AddComponent<BoxCollider>();
                    yield return new WaitForSeconds(0.5f);
                }
                else if (t.name == "Walls")
                {
                    foreach (Transform wall in t)
                    {
                        wall.gameObject.AddComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate; ;
                        yield return new WaitForSeconds(0.3f);
                    }
                }
            }
        }
    }
    Transform hitTransform;
    private void Shoot()
    {
        Vector3 dir = (Vector3.MoveTowards(_damageDealerRB.transform.position, _player.position, _laserSpeed * Time.fixedDeltaTime) - _leftEye.position);
        
        if (shoot)
        {
            Vector3 direction = (Vector3.MoveTowards(_damageDealerRB.transform.position, _player.position, _laserSpeed * Time.fixedDeltaTime) - _leftEye.position);
            Ray ray = new Ray(_leftEye.position, direction);
            
            RaycastHit hit;

            // Perform the Raycast
            if (Physics.Raycast(ray, out hit, 1000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                Vector3 destination = hit.point;
                _leftLaserLR.SetPosition(0, _leftEye.position);
                _leftLaserLR.SetPosition(1, destination);
                
                _rightLaserLR.SetPosition(0, _rightEye.position);
                _rightLaserLR.SetPosition(1, destination);
                Debug.Log("Moving RB");
                _damageDealerRB.MovePosition(destination);
            }
        }
    }
    private void GlowEyes()
    {
        if (glowEyes)
        {
            _leftLight.intensity = Mathf.MoveTowards(_leftLight.intensity, 60f, 60f * Time.deltaTime / _eyeGlowTime);
            _rightLight.intensity = Mathf.MoveTowards(_rightLight.intensity, 60f, 60f * Time.deltaTime / _eyeGlowTime);
        }
    }

    private Vector3 lastPlayerPos;
    IEnumerator ShootLasers()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            _laserAudioSource.Play();
            _leftLaserLR.SetPosition(0, _leftEye.position);
            _rightLaserLR.SetPosition(0, _rightEye.position);
            _leftLaserLR.SetPosition(1, _leftEye.position);
            _rightLaserLR.SetPosition(1, _rightEye.position);            
            glowEyes = true;
            lastPlayerPos = _player.position;
            _damageDealer.gameObject.SetActive(true);
            _damageDealer.isDamaging = false;
            _damageDealerRB.position = lastPlayerPos;
            
            yield return new WaitForSeconds(_eyeGlowTime);
            _leftLaserLR.gameObject.SetActive(true);
            _rightLaserLR.gameObject.SetActive(true);
            _leftLaserLR.SetPosition(1, lastPlayerPos);
            _rightLaserLR.SetPosition(1, lastPlayerPos);
            shoot = true;
            _damageDealer.isDamaging = true;
            yield return new WaitForSeconds(_shootTime);
            _leftLight.intensity = 0f;
            _rightLight.intensity = 0f;
            _leftLaserLR.gameObject.SetActive(false);
            _rightLaserLR.gameObject.SetActive(false);
            _damageDealerRB.gameObject.SetActive(false);
            _damageDealer.isDamaging = false;
            shoot = false;
            glowEyes = false;
            yield return new WaitForSeconds(_cooldownTime);
        }
    }
}
