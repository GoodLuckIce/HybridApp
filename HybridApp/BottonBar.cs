using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace HybridApp
{
    /// <summary>
    ///将状态条单独封装起来。
    ///这种封装方式和封装类似iPhone的SegmentBar不太一样，不是在代码中生成Button。
    ///这里与布局文件相结合。通过inflater布局文件，来得到每个Item。
    /// </summary>
    public class BottonBar : LinearLayout, View.IOnClickListener
    {
        private const int Tag0 = 0;
        private const int Tag1 = 1;
        private const int Tag2 = 2;
        private const int Tag3 = 3;
        private const int Tag4 = 4;
        private Context mContext; 
        private List<View> itemList;
        private int lastButton = -1;

        public BottonBar(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            mContext = context;
            init();
        }
        public BottonBar(Context context)
            : base(context)
        {
            mContext = context;
            init();
        }

        /// <summary>
        /// 得到布局文件中的按钮
        /// </summary>
        private void init()
        {
            LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);
            View layout = inflater.Inflate(Resource.Layout.ButtonBarVw, null);
            layout.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent, 1.0f); 

            Button btnOne = (Button)layout.FindViewById(Resource.Id.btn_item_one);
            Button btnTwo = (Button)layout.FindViewById(Resource.Id.btn_item_two);
            Button btnThree = (Button)layout.FindViewById(Resource.Id.btn_item_three);
            //Button btnfour = (Button)layout.FindViewById(Resource.Id.btn_item_four);
			//Button btnfive = (Button)layout.FindViewById(Resource.Id.btn_item_five);

            btnOne.SetOnClickListener(this);
            btnTwo.SetOnClickListener(this);
            btnThree.SetOnClickListener(this);
            //btnfour.SetOnClickListener(this);
			//btnfive.SetOnClickListener(this);


            btnOne.Tag = Tag0;
            btnTwo.Tag = Tag1;
            btnThree.Tag = Tag2;
            //btnfour.Tag = Tag3;
			//btnfive.Tag = Tag4;

            itemList = new List<View>();
            itemList.Add(btnOne);
            itemList.Add(btnTwo);
            itemList.Add(btnThree);
            //itemList.Add(btnfour);
			//itemList.Add(btnfive);

            this.AddView(layout);
        }

        /// <summary>
        /// 设置默认选中的Item
        /// </summary>
        /// <param name="index"></param>
        public void SetSelectedState(int index)
        {
            if (index != -1 && onItemChangedListener != null)
            {
                if (index > itemList.Count)
                {
                    //throw new RuntimeException("the value of default bar item can not bigger than string array's length");
                }
                itemList[index].Selected = true;
                onItemChangedListener.onItemChanged(index);
                lastButton = index;
            }
        }
        /// <summary>
        /// 恢复未选中状态
        /// </summary>
        /// <param name="index"></param>
        private void SetNormalState(int index)
        {
            if (index != -1)
            {
                if (index > itemList.Count)
                {

                }
                itemList[index].Selected = false;
            }
        }
        /// <summary>
        /// 设置我执行按钮右上角的红色小图标的可见
        /// </summary>
        /// <param name="value"></param>
        public void ShowIndicate(int value)
        {
            //      tvOne.Text = value.ToString();
            //      tvOne.Visibility = ViewStates.Visible;
        }
        /// <summary>
        /// 隐藏 我执行按钮右上角的红色小图标
        /// </summary>
        public void HideIndicate()
        {
            //      tvOne.Visibility = ViewStates.Gone;
        }

        public interface OnItemChangedListener
        {
            void onItemChanged(int index);
        }
        private OnItemChangedListener onItemChangedListener;

        public void SetOnItemChangedListener(OnItemChangedListener onItemChangedListener)
        {
            this.onItemChangedListener = onItemChangedListener;
        }

        #region IOnClickListener 成员

        public void OnClick(View v)
        {
            int tag = (int)v.Tag;
            switch (tag)
            {
                case Tag0:
                    SetNormalState(lastButton);
                    SetSelectedState(tag);
                    break;
                case Tag1:
                    SetNormalState(lastButton);
                    SetSelectedState(tag);
                    break;
                case Tag2:
                    SetNormalState(lastButton);
                    SetSelectedState(tag);
                    break;
                case Tag3:
                    SetNormalState(lastButton);
                    SetSelectedState(tag);
                    break;
                case Tag4:
                    SetNormalState(lastButton);
                    SetSelectedState(tag);
                    break;
            }
        }

        #endregion


    }

}