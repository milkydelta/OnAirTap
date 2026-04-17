enum LIVnyan_cfg {
    CAM_ON = 0x01,
    LOG_ON = 0x02,
    LOGSPM = 0x04
};

typedef struct _LIVnyan_dat {
    float x;
    float y;
    float z;
    float qw;
    float qx;
    float qy;
    float qz;
    float fov;
    int cfg;
} LIVnyan_dat;

typedef struct _dataBlock{
    char* name;
    int length;
    void* data;
    int fd;
} dataBlock;
