using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerInterface;


namespace ControllerLib
{
    class Controller : PlayerInterface.IControl
    {
        private int[] selfPosition = new int[2]; //存储控制角色的位置信息
        private int[] mapStatus = new int[4]; //存储角色四周的位置信息
        private int[,] playerPosition = new int[4,2];//存储对手位置信息
        private int[,] Map = new int[14, 14];//存储全局地图信息
        int beginx, endx, beginz, endz;//防越界

        public string GetTeamName()
        {
            return "瞅你咋滴";
        }


        //获取敌人位置
        private void GetPlayerPosition( IEntity entity )
        {
            //获取对手位置信息
            int myIndex = entity.GetIndex();
            int[] temp = new int[2];
            int i;
            for ( i = 0; i < 4; i++ )
            {
                if (i == myIndex)
                {
                    playerPosition[i, 0] = selfPosition[0];
                    playerPosition[i, 1] = selfPosition[1];
                    break;
                }
                temp = entity.PlayerPosition(i);
                playerPosition[i, 0] = temp[0];
                playerPosition[i, 1] = temp[1];
            }
        }

        //判断当前位置放置炸弹是否安全
        private int TryBomb(IEntity entity, int[] selfPosition)
        {

            return -1;
        }
        
        //防越界
        private void Bound(int i, int j)
        {
            if (i == 0)//边界上
            {
                beginx = i;
                endx = i + 1;
            }
            else if (i < 13)
            {
                beginx = i - 1;
                endx = i + 1;
            }
            else
            {
                beginx = i - 1;
                endx = i;
            }

            if (j == 0)//边界上
            {
                beginz = j;
                endz = j + 1;
            }
            else if (j < 13)
            {
                beginz = j - 1;
                endz = j + 1;
            }
            else
            {
                beginz = j - 1;
                endz = j;
            }
        }

        //移动类
        //获取四周信息
        private void GetMap( IEntity entity, int[] selfPosition)
        {
            int?[] temp = new int?[4];
            temp[0] = entity.GetMapType( selfPosition[0] + 1, selfPosition[1] );
            temp[1] = entity.GetMapType( selfPosition[0] - 1, selfPosition[1] );
            temp[2] = entity.GetMapType( selfPosition[0], selfPosition[1] + 1 );
            temp[3] = entity.GetMapType( selfPosition[0], selfPosition[1] - 1 );
            //int?转换至int，存入角色四周的位置信息mapStatus
            for ( int i = 0; i < 4; i++ )
            {
                if ( temp[i].HasValue )
                {
                    mapStatus[i] = temp[i].Value;
                }
                else
                {
                    mapStatus[i] = -1;
                }
            }

        }

        //获取安全区域
        private void SaveRegion(IEntity entity, int[] selfPosition)
        {
            //计算安全区域，存储在MapTemp中
            int? temp;
            int i, j;
            

            //置0
            for (i = 0; i < 14; i++)
            {
                for (j = 0; j < 14; j++)
                {
                    Map[i, j] = 0;
                }
            }

            //计算全局地图type
            for (i = 0; i < 14; i++)
            {
                for (j = 0; j < 14 ; j++)
                {
                    temp = entity.GetMapType(i,j);//获取(i,j)的type
                    if ( temp.HasValue )
                    {
                        if ( Map[i, j] != 0 )
                        {
                            continue;
                        }
                        Map[i, j] = temp.Value;
                        if (temp.Value == 3)//有炸弹的周围一圈都是危险
                        {
                            Bound(i, j);
                            for (int x = beginx; x <= endx; x++)
                            {
                                for (int z = beginz; z <= endz; z++)
                                {
                                    Map[x, z] = 3;
                                }
                            }
                        }
                    }
                    else
                    {
                        Map[i, j] = -1;
                    }
                }
            }
            
        }


        //寻找路径
        private void FindPath(IEntity entity, int[] selfPosition)
        {

        }

        //移动
        private bool Move(IEntity entity)
        {
            //第一步 选定方向
            int direction;
            for (direction = 0; direction < 4; direction++)
            {
                if (mapStatus[direction] == 0)//角色可到达
                {
                    break;
                }
            }

            //第二步 移动
            switch (direction)
            {
                case 0:
                    return entity.MoveEast();
                case 1:
                    return entity.MoveWest();
                case 2:
                    return entity.MoveSouth();
                case 3:
                    return entity.MoveNorth();
                default: return false;
            }
        }

        //攻击类


        //buff类

        //更新
        public void Update(IEntity entity)
        {
            // TODO: 逻辑写入位置
            float remainTime;

            remainTime = entity.GetRemainingTime();

            selfPosition = entity.GetPosition();    //获取角色位置
            GetPlayerPosition(entity);              //获取敌人位置
            SaveRegion(entity, selfPosition);       //获取地图信息
            GetMap(entity, selfPosition);           //获取可移动方向

            if (!entity.IsMoving())
            {
                Move(entity); //选定方向并移动
            }
            // ...更多代码
            return;
        }

    }
}
