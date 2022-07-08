using Controller;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

public class ShipView : MonoBehaviour
{
    [SerializeField] private ShipCollider[] _colliders;
    [SerializeField] private GameObject _deathFx;
    [SerializeField] private GameObject _body;

    public Action<Collider2D> OnDamage;
    private SettingsConfig _settings;

    internal void Init(SettingsConfig settings) {
        _settings = settings;

        transform.localScale = Vector3.one;
        foreach (var c in _colliders) {
            c.OnCollision += OnColision;
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

    private void OnColision(Collider2D other) {
        OnDamage(other);
    }

    public BaseWeapon AddWeapon(BaseWeapon weapon) {
        return Instantiate(weapon, transform);
    }
    internal void Tick(float deltaTime) {

    }

}