using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M1911A : MonoBehaviour {

    public ShootParticle shootParticle;
    public Bullet bullet;
    public FireSound fireSound;
    public MisfireSound misfireSound;

    private GameObject hammer = null;
    private GameObject slider = null;

    private Vector3 hammerDefaultPosition = Vector3.zero;
    private Vector3 hammerDefaultRotation = Vector3.zero;
    private Vector3 sliderDefaultPosition = Vector3.zero;
    private Vector3 sliderDefaultRotation = Vector3.zero;
    
    private bool canFire = true;

    void Start() {
        hammer = gameObject.transform.Find("M1911A/Hammer").gameObject;
        slider = gameObject.transform.Find("M1911A/Slider").gameObject;

        hammerDefaultPosition = hammer.transform.localPosition;
        hammerDefaultRotation = hammer.transform.localEulerAngles;
        sliderDefaultPosition = slider.transform.localPosition;
        sliderDefaultRotation = slider.transform.localEulerAngles;
        
    }

    private IEnumerator TestCorutine() {

        yield return new WaitForSeconds(0.3f);

        while (true) {
            StartCoroutine(ShootCorutine());

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator ShootCorutine() {
        float delay = 0;
        float delta = 0;

        float maxSliderPosX = -0.14f;
        float maxHammerRotY = -75f;

        bool loop = true;

        Vector3 rotation = gameObject.transform.eulerAngles;

        loop = false;
        delta = 0.5f;
        delay = 0.001f;
        for (float i = 1; loop; i -= delta) {
            if (i < 0) {
                i = 0;
                loop = false;
            }
            hammer.transform.localEulerAngles = new Vector3(hammerDefaultRotation.x, hammerDefaultRotation.y + i * maxHammerRotY, hammerDefaultRotation.z);

            yield return new WaitForSeconds(delay);
        }
        fireSound.Spawn(gameObject.transform.position);
        ShootParticle shtPtc = shootParticle.Spawn(gameObject.transform.position + gameObject.transform.forward * 0.25f + gameObject.transform.up * 0.124f);
        shtPtc.SetRotation(rotation);
        //shtPtc.gameObject.transform.SetParent(gameObject.transform); 오류남. 왠지 모르겠음.

        loop = true;
        delta = 0.3f;
        delay = 0.005f;
        for (float i = 0;loop;i += delta) {
            if (i > 1) {
                i = 1;
                loop = false;
            }
            slider.transform.localPosition = new Vector3(sliderDefaultPosition.x + i * maxSliderPosX, sliderDefaultPosition.y, sliderDefaultPosition.z);
            hammer.transform.localEulerAngles = new Vector3(hammerDefaultRotation.x, hammerDefaultRotation.y + i * maxHammerRotY, hammerDefaultRotation.z);

            yield return new WaitForSeconds(delay);
        }

        Bullet blt = bullet.Spawn(gameObject.transform.position + gameObject.transform.forward * 0.045f + gameObject.transform.up * 0.12f);
        blt.SetRotation(rotation);
        Rigidbody bulletRb = blt.GetRigidbody();
        bulletRb.AddForce(gameObject.transform.up * Random.Range(0.24f, 0.36f) + gameObject.transform.right * Random.Range(2f, 3f), ForceMode.VelocityChange);
        bulletRb.AddRelativeTorque(new Vector3(0, Random.Range(4f, 6f), 0), ForceMode.VelocityChange);
        
        loop = true;
        delta = 0.3f;
        delay = 0.005f;
        for (float i = 1;loop;i -= delta) {
            if (i < 0) {
                i = 0;
                loop = false;
            }
            slider.transform.localPosition = new Vector3(sliderDefaultPosition.x + i * maxSliderPosX, sliderDefaultPosition.y, sliderDefaultPosition.z);
            
            yield return new WaitForSeconds(delay);
        }


        yield return null;
    }

    public void Shoot() {
        StartCoroutine(ShootCorutine());
    }

    public void Misfire() {
        misfireSound.Spawn(gameObject.transform.position);
    }

    /*

    private IEnumerator TmpCorutine() {
        yield return new WaitForSeconds(1);

        Shoot();
        
        while (true) {
            yield return new WaitForSeconds(3);
            Shoot();
        }
    }

    private IEnumerator ReloadCorutine() {
        float deltaTime = 0.005f;
        float hammerDeltaRotY = -75f;
    }

    private IEnumerator ShootCorutine() {
        float deltaTime = 0.005f;
        float sliderDeltaX = 0.14f;

        float delta = 0;
        shootParticle.Spawn(gameObject.transform.position + gameObject.transform.forward * 0.9f + gameObject.transform.up * 0.4f);
        bullet.Spawn(gameObject.transform.position + gameObject.transform.forward * 0.15f + gameObject.transform.up * 0.4f);

        while (true) {
            delta += 0.3f;
            if (delta > 1) {
                delta = 1f;
                slider.transform.localPosition = new Vector3(sliderDefaultPosition.x - delta * sliderDeltaX, sliderDefaultPosition.y, sliderDefaultPosition.z);
                break;
            }

            slider.transform.localPosition = new Vector3(sliderDefaultPosition.x - delta * sliderDeltaX, sliderDefaultPosition.y, sliderDefaultPosition.z);

            yield return new WaitForSeconds(deltaTime);
        }

        delta = 1;

        while (true) {
            delta -= 0.3f;
            if (delta < 1) {
                delta = 0f;
                slider.transform.localPosition = new Vector3(sliderDefaultPosition.x - delta * sliderDeltaX, sliderDefaultPosition.y, sliderDefaultPosition.z);
                break;
            }

            slider.transform.localPosition = new Vector3(sliderDefaultPosition.x - delta * sliderDeltaX, sliderDefaultPosition.y, sliderDefaultPosition.z);

            yield return new WaitForSeconds(deltaTime);
        }
    }

    public void Shoot() {
        if (canFire) {
            StartCoroutine(ShootCorutine());
        }
    }*/

    
}
