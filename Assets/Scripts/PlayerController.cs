using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] LayerMask jumpableLayer, carLayer;
    [SerializeField] GameObject jumpMarker;
    [SerializeField] Animator anim;

    public CarController currentCar, targetCar;
    [SerializeField] float carDetectionRange, carJumpRange;
    [SerializeField] float slowmoSpeed, variationSpeed;

    bool slowmo, jumping;
  
    private void Update()
    {
        JumpControl();

        if (currentCar != null)
            transform.position = currentCar.playerSpot.position;

        if(jumpMarker.activeInHierarchy)
        {
            Vector3 levelPos = new Vector3(jumpMarker.transform.position.x, transform.position.y, jumpMarker.transform.position.z);
            transform.LookAt(levelPos);
        }
    }

    void JumpControl()
    {
        if (Input.GetButtonDown("Fire1") && !jumping)
            slowmo = true;
        if (Input.GetButtonUp("Fire1"))
        {
            float d = (transform.position - jumpMarker.transform.position).magnitude;

            if (d <= carJumpRange && !jumping)
                Jump();

            slowmo = false;
        }

        if (slowmo)
        {
            PositionMarker();
            targetCar = PickTargetCar();

            if (Time.timeScale != slowmoSpeed)
                Time.timeScale = Mathf.Lerp(Time.timeScale, slowmoSpeed, variationSpeed * Time.deltaTime);
        }
        else
        {
            if (jumpMarker.activeInHierarchy)
                jumpMarker.SetActive(false);

            if (Time.timeScale != 1)
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1, variationSpeed * Time.deltaTime);
        }
    }

    void PositionMarker()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, jumpableLayer))
        {
            if (!jumpMarker.activeInHierarchy)
                jumpMarker.SetActive(true);

            jumpMarker.transform.position = hit.point;
        }
        else
        {
            jumpMarker.transform.position = transform.position;
            jumpMarker.SetActive(false);
        }
    }

    CarController PickTargetCar()
    {
        if (Physics.CheckSphere(jumpMarker.transform.position, carDetectionRange, carLayer))
        {
            Collider[] carsNearby = Physics.OverlapSphere(jumpMarker.transform.position, carDetectionRange, carLayer);

            if (carsNearby[0].GetComponent<CarController>() != currentCar)
                return carsNearby[0].GetComponent<CarController>();
            else
                return null;
        }
        else
            return null;
    }

    void Jump()
    {
        if (targetCar != null)
        {
            JumpToCar(targetCar);
        }
    }

    public void JumpToCar(CarController car)
    {
        jumping = true;
        anim.SetBool("Jumping", jumping);

        transform.SetParent(car.transform);

        Invoke("EndJumpState", 1.1f);

        transform.DOLocalJump(car.playerSpot.localPosition, 3, 1, 1.3f).OnComplete(() => { currentCar.hostingPlayer = false; currentCar = targetCar; currentCar.hostingPlayer = true; targetCar = null; });
    }

    void EndJumpState()
    {
        jumping = false;
        anim.SetBool("Jumping", jumping);
    }
}
