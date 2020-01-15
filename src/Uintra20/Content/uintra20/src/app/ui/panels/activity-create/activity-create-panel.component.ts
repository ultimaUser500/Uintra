import { Component, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { IActivityCreatePanel } from './activity-create-panel.interface';
import { ITagData } from 'src/app/feature/project/reusable/inputs/tag-multiselect/tag-multiselect.interface';
import { DropzoneComponent } from 'ngx-dropzone-wrapper';
import { MAX_LENGTH } from './_constants.js';
import { CreateSocialService } from 'src/app/services/createActivity/create-social.service';
import { IUserAvatar } from 'src/app/feature/project/reusable/ui-elements/user-avatar/user-avatar-interface';
import ParseHelper from 'src/app/feature/shared/helpers/parse.helper';
import { parse } from 'querystring';
import { ModalService } from 'src/app/services/general/modal.service';

@Component({
  selector: 'activity-create-panel',
  templateUrl: './activity-create-panel.component.html',
  styleUrls: ['./activity-create-panel.component.less'],
  encapsulation: ViewEncapsulation.None
})
export class ActivityCreatePanel implements OnInit {
  @ViewChild('dropdownRef', { static: false }) dropdownRef: DropzoneComponent;
  data: IActivityCreatePanel;
  availableTags: Array<ITagData> = [];
  isPopupShowing = false;
  files: Array<any> = [];
  tags: Array<ITagData> = [];
  description = '';
  inProgress = false;
  userAvatar: IUserAvatar;

  get isSubmitDisabled() {
    if (ParseHelper.stripHtml(this.description).length > MAX_LENGTH || this.inProgress) {
      return true;
    }
    return !this.description && this.files.length === 0;
  }

  constructor(private socialContentService: CreateSocialService, private modalService: ModalService) { }

  ngOnInit() {
    const parsed = ParseHelper.parseUbaselineData(this.data);
    this.availableTags = Object.values(
      JSON.parse(JSON.stringify(this.data.tags.get().userTagCollection))
    );
    this.userAvatar = {
      name: parsed.creator.displayedName,
      photo: parsed.creator.photo
    };
  }

  onShowPopUp() {
    this.showPopUp();
  }
  onHidePopUp() {
    if (this.description || this.tags.length || this.files.length) {
      if (confirm('Are you sure?')) {
        this.resetForm();
        this.hidePopUp();
      }
    } else {
      this.hidePopUp();
    }
  }

  hidePopUp() {
    this.modalService.removeClassFromRoot('disable-scroll');
    this.isPopupShowing = false;
  }

  showPopUp() {
    this.modalService.addClassToRoot('disable-scroll');
    this.isPopupShowing = true;
  }

  addAttachment() {
    this.dropdownRef.directiveRef.dropzone().clickableElements[0].click();
  }

  onUploadSuccess(fileArray: Array<any> = []): void {
    this.files.push(fileArray);
  }

  onFileRemoved(removedFile: object) {
    this.files = this.files.filter(file => {
      const fileElement = file[0];
      return fileElement !== removedFile;
    });
  }

  getMediaIdsForResponse() {
    return this.files.map(file => file[1]).join(';');
  }
  getTagsForResponse() {
    return this.tags.map(tag => tag.id);
  }

  resetForm(): void {
    this.files = [];
    this.tags = [];
    this.description = '';
  }

  onSubmit() {
    this.inProgress = true;
    this.socialContentService
      .submitSocialContent({
        description: this.description,
        OwnerId: 'cb6969e1-ac68-4cae-88a3-8b1cbc453ef7',
        NewMedia: this.getMediaIdsForResponse(),
        TagIdsData: this.getTagsForResponse()
      })
      .then(response => {
        this.hidePopUp();
        this.socialContentService.refreshFeed();
      })
      .catch(err => {
        this.hidePopUp();
      })
      .finally(() => {
        this.inProgress = false;
        this.resetForm();
      });
  }
}
