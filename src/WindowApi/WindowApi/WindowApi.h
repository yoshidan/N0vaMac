//
//  WindowApi.h
//  WindowApi
//
//  Created by yoshida on 2022/01/28.
//

#ifndef WindowApi_h
#define WindowApi_h

#ifdef __cplusplus
extern "C" {
#endif

void background(void);
    
void hideTitleBar(void);

void initializePosition(void);

void maximize(void);

#ifdef __cplusplus
}
#endif

#endif /* WindowApi_h */
