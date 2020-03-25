import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AS_DYNAMIC_COMPONENT } from '@ubaseline/next';
import { UserListPanel } from './user-list-panel.component';
import { TranslateModule } from '@ngx-translate/core';
import { UserAvatarModule } from 'src/app/feature/reusable/ui-elements/user-avatar/user-avatar.module';
import { UlinkModule } from 'src/app/shared/pipes/link/ulink.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [UserListPanel],
  imports: [
    CommonModule,
    TranslateModule,
    UserAvatarModule,
    RouterModule,
    UlinkModule,
  ],
  providers: [{provide: AS_DYNAMIC_COMPONENT, useValue: UserListPanel}],
  entryComponents: [UserListPanel]
})
export class UserListPanelModule {}
