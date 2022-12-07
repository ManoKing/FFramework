using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{

    public class ItemData
    {
        public int mId;
        public string mName;
        public int mFileSize;
        public string mDesc;
        public int mIcon;
        public int mStarCount;
        public bool mChecked;
        public bool mIsExpand;
    }

    public class DataSourceMgr : MonoBehaviour
    {

        List<ItemData> mItemDataList = new List<ItemData>();
        System.Action mOnRefreshFinished = null;
        System.Action mOnLoadMoreFinished = null;
        int mLoadMoreCount = 20;
        float mDataLoadLeftTime = 0;
        bool mIsWaittingRefreshData = false;
        bool mIsWaitLoadingMoreData = false;
        public int mTotalDataCount = 10000;

        static DataSourceMgr instance = null;

        public static DataSourceMgr Get
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindObjectOfType<DataSourceMgr>();
                }
                return instance;
            }

        }

        void Awake()
        {
            Init();
        }


        public void Init()
        {
            DoRefreshDataSource();
        }

        public ItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= mItemDataList.Count)
            {
                return null;
            }
            return mItemDataList[index];
        }

        public ItemData GetItemDataById(int itemId)
        {
            int count = mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                if(mItemDataList[i].mId == itemId)
                {
                    return mItemDataList[i];
                }
            }
            return null;
        }

        public int TotalItemCount
        {
            get
            {
                return mItemDataList.Count;
            }
        }

        public void RequestRefreshDataList(System.Action onReflushFinished)
        {
            mDataLoadLeftTime = 1;
            mOnRefreshFinished = onReflushFinished;
            mIsWaittingRefreshData = true;
        }

        public void RequestLoadMoreDataList(int loadCount,System.Action onLoadMoreFinished)
        {
            mLoadMoreCount = loadCount;
            mDataLoadLeftTime = 1;
            mOnLoadMoreFinished = onLoadMoreFinished;
            mIsWaitLoadingMoreData = true;
        }

        public void Update()
        {
            if (mIsWaittingRefreshData)
            {
                mDataLoadLeftTime -= Time.deltaTime;
                if (mDataLoadLeftTime <= 0)
                {
                    mIsWaittingRefreshData = false;
                    DoRefreshDataSource();
                    if (mOnRefreshFinished != null)
                    {
                        mOnRefreshFinished();
                    }
                }
            }
            else if (mIsWaitLoadingMoreData)
            {
                mDataLoadLeftTime -= Time.deltaTime;
                if (mDataLoadLeftTime <= 0)
                {
                    mIsWaitLoadingMoreData = false;
                    DoLoadMoreDataSource();
                    if (mOnLoadMoreFinished != null)
                    {
                        mOnLoadMoreFinished();
                    }
                }
            }

        }

        public void SetDataTotalCount(int count)
        {
            mTotalDataCount = count;
            DoRefreshDataSource();
        }

        void DoRefreshDataSource()
        {
            mItemDataList.Clear();
            for (int i = 0; i < mTotalDataCount; ++i)
            {
                ItemData tData = new ItemData();
                tData.mId = i;
                tData.mName = "Item" + i;
                tData.mDesc = "Item Desc For Item " + i;
                tData.mIcon = Random.Range(0, 24);
                tData.mStarCount = Random.Range(0, 6);
                tData.mFileSize = Random.Range(20, 999);
                tData.mChecked = false;
                tData.mIsExpand = false;
                mItemDataList.Add(tData);
            }
        }

        void DoLoadMoreDataSource()
        {
            int count = mItemDataList.Count;
            for (int k = 0; k < mLoadMoreCount; ++k)
            {
                int i = k + count;
                ItemData tData = new ItemData();
                tData.mId = i;
                tData.mName = "Item" + i;
                tData.mDesc = "Item Desc For Item " + i;
                tData.mIcon = Random.Range(0, 24);
                tData.mStarCount = Random.Range(0, 6);
                tData.mFileSize = Random.Range(20, 999);
                tData.mChecked = false;
                tData.mIsExpand = false;
                mItemDataList.Add(tData);
            }
        }

        public void CheckAllItem()
        {
            int count = mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                mItemDataList[i].mChecked = true;
            }
        }

        public void UnCheckAllItem()
        {
            int count = mItemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                mItemDataList[i].mChecked = false;
            }
        }

        public bool DeleteAllCheckedItem()
        {
            int oldCount = mItemDataList.Count;
            mItemDataList.RemoveAll(it => it.mChecked);
            return (oldCount != mItemDataList.Count);
        }

    }

}