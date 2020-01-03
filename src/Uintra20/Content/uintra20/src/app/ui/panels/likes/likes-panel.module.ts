import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AS_DYNAMIC_COMPONENT } from '@ubaseline/next';
import { LikesPanel } from './likes-panel.component';
import { LikeButtonModule } from 'src/app/feature/project/reusable/ui-elements/like-button/like-button.module';

@NgModule({
  declarations: [LikesPanel],
  imports: [
    CommonModule,
    LikeButtonModule
  ],
  providers: [{provide: AS_DYNAMIC_COMPONENT, useValue: LikesPanel}],
  entryComponents: [LikesPanel]
})
export class LikesPanelModule {}
