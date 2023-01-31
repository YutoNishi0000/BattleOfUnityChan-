using ExitGames.Demos.DemoAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    #region Public Properties

    //�L�����̓���ɏ��悤�ɒ������邽�߂�Offset
    public Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

    //�v���C���[���O�ݒ�pText
    public Text PlayerNameText;

    //�v���C���[��HP�pSlider
    public Slider PlayerHPSlider;

    //�v���C���[�̃`���b�g�pText
    //public Text PlayerChatText;

    #endregion

    #region Private Properties
    //�Ǐ]����L������PlayerManager���
    PlayerManager _target;
    float _characterControllerHeight;
    Transform _targetTransform;
    Vector3 _targetPosition;
    #endregion

    #region MonoBehaviour Messages

    void Awake()
    {
        //���̃I�u�W�F�N�g��Canvas�I�u�W�F�N�g�̎q�I�u�W�F�N�g�Ƃ��Đ���
        this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
    }

    void Update()
    {
        //����Player�����Ȃ��Ȃ����炱�̃I�u�W�F�N�g���폜
        if (_target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        // ���݂�HP��Slider�ɓK�p
        if (PlayerHPSlider != null)
        {
            PlayerHPSlider.value = _target.HP;
        }

        // ����`���b�g��\��
        //if (PlayerChatText != null)
        //{
        //    PlayerChatText.text = _target.ChatText;
        //}

    }

    #endregion

    void LateUpdate()
    {
        //target�̃I�u�W�F�N�g��ǐՂ���
        if (_targetTransform != null)
        {
            _targetPosition = _targetTransform.position;    //�O������ԏ��target�̍��W�𓾂�
            _targetPosition.y += _characterControllerHeight;  //�L�����N�^�[�̔w�̍������l������
                                                              //target�̍��W���瓪��UI�̉�ʏ�̓񎟌����W���v�Z���Ĉړ�������
            this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + ScreenOffset;
        }
    }

    #region Public Methods
    public void SetTarget(PlayerManager target)
    {
        if (target == null)//target�����Ȃ���΃G���[��Console�ɕ\��
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        //target�̏������̃X�N���v�g���Ŏg���̂ŃR�s�[
        _target = target;
        _targetTransform = _target.GetComponent<Transform>();

        CharacterController _characterController = _target.GetComponent<CharacterController>();

        //PlayerManager�̓���UI�ɕ\���������f�[�^���R�s�[
        if (_characterController != null)
        {
            _characterControllerHeight = _characterController.height;
        }

        //if (PlayerNameText != null)
        //{
        //    PlayerNameText.text = _target.photonView.owner.NickName;
        //}
        if (PlayerHPSlider != null)
        {
            PlayerHPSlider.value = _target.HP;
        }
        //if (PlayerChatText != null)
        //{
        //    PlayerChatText.text = _target.ChatText;
        //}
    }
    #endregion
}
