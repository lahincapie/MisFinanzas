import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

interface MessageDialogData {
  title: string;
  message: string;
  icon?: string;
}

@Component({
  selector: 'app-message-dialog',
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './message-dialog.html',
  styleUrl: './message-dialog.css'
})
export class MessageDialog {
  data = inject<MessageDialogData>(MAT_DIALOG_DATA);
}