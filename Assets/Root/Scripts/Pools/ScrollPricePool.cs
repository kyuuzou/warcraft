using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollPricePool : Pool<ScrollPricePool, ScrollPrice> {

    private AudioManager audioManager;

    public override ScrollPrice GetInstance () {
        ScrollPrice instance = base.GetInstance ();

        instance.SetOpacity (1.0f);

        return instance;
    }

    public override void InitializeExternals () {
        if (this.InitializedExternals) {
            return;
        }

        base.InitializeExternals ();

        this.audioManager = ServiceLocator.Instance.AudioManager;
    }

    public void Scroll (Transform parent, int cost, bool successfulPurchase) {
        this.StartCoroutine (this.ScrollCoroutine (parent, cost, successfulPurchase));
    }

    private IEnumerator ScrollCoroutine (Transform parent, int cost, bool successfulPurchase) {
        ScrollPrice price = this.GetInstance ();
        price.SetText (cost.ToString ());
        price.SetColor (successfulPurchase ? Color.white : Color.red);
        price.transform.parent = parent;
        price.transform.SetLocalPosition (0.0f, 25.5f, - 100.0f);

        this.audioManager.Play (
            successfulPurchase ? AudioIdentifier.PurchaseSuccessful : AudioIdentifier.PurchaseFailed
        );

        float delta = - 1.0f;

        do {
            price.transform.SetLocalY (price.transform.localPosition.y + Time.deltaTime * 50.0f);

            price.SetOpacity (1.0f - delta);

            delta += Time.deltaTime;
            yield return null;
        } while (delta < 1.0f);

        this.AddInstance (price);
    }
}
