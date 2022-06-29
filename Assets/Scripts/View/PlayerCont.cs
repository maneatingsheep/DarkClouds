using Controller;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

public class PlayerCont : MonoBehaviour
{
    [SerializeField] private ShipCollider[] _colliders;
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _deathFx;

    public Action<Collider2D> OnColision;
    private SettingsConfig _settings;

    internal void Init(SettingsConfig settings) {
        _settings = settings;

        transform.localScale = Vector3.one;
        foreach (var c in _colliders) {
            c.OnCollision += Colision;
        }

        //make ship collider ignore each other
        for (int i = 0; i < _colliders.Length - 1; i++) {
            for (int j = i + 1; j < _colliders.Length; j++) {
                Physics2D.IgnoreCollision(_colliders[i].GetComponent<Collider2D>(), _colliders[j].GetComponent<Collider2D>(), true);

            }
        }

    }

    internal void ResetView() {
    }

    internal void StartGame() {
        _body.SetActive(true);
        _deathFx.SetActive(false);
    }

    internal void GameOver() {
        _body.SetActive(false);
        _deathFx.SetActive(true);
    }

    private void Colision(Collider2D other) {
        OnColision(other);
    }

    public BaseWeapon AddWeapon(BaseWeapon weapon) {
        return Instantiate(weapon, transform);
    }
    internal void Tick(float deltaTime) {


    }

}
