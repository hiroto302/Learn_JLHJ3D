using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;  // 移動ベクトル
    Quaternion m_Rotation = Quaternion.identity;  // 回転変数 初期化ではidentity(インスタンスされるオブジェクトのデフォルト値)を指定
    public float turnSpeed = 20f; // 回転速度
    void Start()
    {
        /*
        GetComponentメソッド MonoBehaviourクラスのメソッドでジェネリックメソッドである
        2 つの異なるパラメーターセット（標準のパラメーター と 型パラメーター）を持つ。山括弧の間に指定されたパラメーターは、型パラメーター
        */
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }
    /*
    物理演算とアニメーションの間の衝突を避けるために、アニメーターが物理演算のループと同期して実行されるようにした。
    ただし、ここでは OnAnimatorMove を使ってルートモーションをオーバーライドしている。
    つまり、OnAnimatorMove は Update メソッドのようなレンダリングではなく、実際には物理演算に合わせて呼び出されることになる。そのため、
    移動ベクトルと回転を Update で設定するとOnAnimatorMove が最初に呼ばれた場合は、値が設定されていないクォータニオンは全く意味を成さないので、問題が発生してしまう。
    OnAnimatorMove に合わせて移動ベクトルと回転が適切に設定されるようにするために、Update メソッドを以下のように FixedUpdate メソッドに変更した
    */
    void FixedUpdate()
    {
        // 入力
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();
        // 水平入力されているかどうか
        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f); // 水平がほぼ 0 になっていないときに hasHorizontalInput が true を返す
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        // walk状態であるか
        bool isWalking = hasHorizontalInput || hasVerticalInput; // hasHorizontalInput または hasVerticalInput が true の場合は isWalking は true で、そうでない場合は false
        m_Animator.SetBool ("IsWalking", isWalking);
        // 回転
        /*
        RotateTowardsメソッド(回転させるベクトル, 回転方向, 第一引数と第二引数に割り当てたベクトルに対する角度の変化量(回転させる角度), 大きさの変化(magunit))
        */
        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward); // LookRotationメソッド 指定されたパラメーターの方向を向いた回転を作成

        // 足音の再生
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }
    }

    /*
    「リジッドボディがアタッチされているアニメーションキャラクターを動かす方法について」
    キャラクターに Walk アニメーションが含まれている時、それにはルートモーションを適用すれは思い描く挙動になるはず。
    しかし、このアニメーションには回転がない！
    Update メソッドでリジッドボディを回転させようとすると、アニメーションでオーバーライドされる可能性が生まれる
    （animtorのコンポーネントであるアニメーションによるルートモーションを利用したキャラクター移動は、通常であればUpdateで処理される。
        だから、アニメーション挙動と物理挙動の両方をupdateメッド内で競合させると、キャラクターが回転が必要な時に回転しない可能性が生じる)
    （回転処理のために？）実際に必要なのは、アニメーション(walkのanimation?)のルートモーションの一部だが、すべてではない。
    具体的には、回転ではなく動きを適用する必要がある。では、アニメーターからルートモーションの適用方法を変更するにはどうすればいいのかというと、幸いにも、MonoBehaviour には、アニメーターからルートモーションの適用方法を変更するための特別なメソッドが用意されている。
    ルートモーションを修正するアニメーション動作を処理するコールバックである
    void OnAnimatorMove(){};
    このコールバックは、ステートマシンとアニメーションが評価された後、OnAnimatorIKの前に、各フレームで呼び出される.(だから、animator.deltaPostionが取得できる！)
    アニメーターコンポーネントは OnAnimatorMove があるスクリプトを検知して、 Apply Root Motion プロパティーに Handled by Script と表示されます。
    */
    void OnAnimatorMove()
    {
        // 移動
        /*
        新しい位置はリジッドボディの現在の位置と、
        そこにアニメーターの deltaPosition(Vector3型 最後に使用したフレームでアバターの差分を取得します) の大きさを掛け合わせることにより、移動ベクトルに変更を加える。
        アニメーターの deltaPosition は、このフレームに適用されていた場合のルートモーションによる位置の移動量。
        その大きさ（ベクトルの長さ）を取得し、実際にキャラクターに動いて欲しい方向にある移動ベクトルを掛け合わせる！
        */
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        // 回転
        m_Rigidbody.MoveRotation (m_Rotation);
    }
}
